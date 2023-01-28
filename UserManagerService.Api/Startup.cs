using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OBS.UserManagementService.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UserManagerService.Api.MiddleWares;
using UserManagerService.Entities;
using UserManagerService.Interfaces.Repositories;
using UserManagerService.Repository;
using UserManagerService.Services;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Helpers;
using UserManagerService.Shared.Hubs;
using UserManagerService.Shared.Interfaces.Helpers;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Interfaces.Shared;
using UserManagerService.Shared.Settings;

namespace UserManagerService
{
    public class Startup
    {
        private readonly IWebHostEnvironment Environment;
        private readonly ILogger<Startup> Logger;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Environment = environment;
            Logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            if (Environment.IsDevelopment())
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                }));
            else
            {
                var connectionString = Configuration.GetConnectionString("MySqlConnection");
                services.AddDbContext<ApplicationDbContext>(options =>
                           options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 29)),
                           options => options.EnableRetryOnFailure(3)));
            }

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
             .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            //services.AddRazorPages();
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            }).AddRazorRuntimeCompilation().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            //register the initializer
            services.AddAsyncInitializer<Initializer>();

            services.AddSignalR(options =>
            {
                //options.KeepAliveInterval = TimeSpan.FromDays(30);
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(1);
            });
            // registers Repository services


            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<SimpleRoleService>();
            services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            services.AddScoped<IApiService, ApiService>();

            services.AddSingleton<IHttpOrchestrator, HttpOrchestrator>();
            services.AddScoped<IFileManagerHelper, FileManagerHelper>();

            // registers AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddScoped<RoleManager<Role>>();
            services.AddScoped<UserManager<User>>();
            services.AddScoped<SignInManager<User>>();

            services.Configure<WebProtocolSettings>(Configuration.GetSection("WebProtocolSettings"));
            var protocols = Configuration.GetSection("WebProtocolSettings").Get<WebProtocolSettings>();

            services.AddSingleton<IAuthHelper, AuthHelper>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,// After discussion, we are not going to use it for now
                    ValidateAudience = false, // Same here
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(protocols.EncryptionKey)),
                    //AuthenticationType = "JWT" // Might not be important
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["Authentication"];
                        return Task.CompletedTask;
                    }
                };
            });

            // check between scoped and transcient
            services.AddScoped<IUserContext, UserContext>(c =>
            {
                //TODO Handle incorrect cases.
                IHttpContextAccessor httpContextAccessor = c.GetService<IHttpContextAccessor>();


                ClaimsPrincipal claimsPrincipal = httpContextAccessor.HttpContext?.User;

                if (claimsPrincipal == null)
                    return new UserContext();

                var claims = ((ClaimsIdentity)claimsPrincipal.Identity).Claims;

                var userId = claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => Guid.Parse(c.Value)).FirstOrDefault();

                var jwt = httpContextAccessor.HttpContext.Request.Cookies["Authentication"];

                if (userId == Guid.Empty)
                {
                    //Check again from the cookie

                    if (string.IsNullOrEmpty(jwt))
                        return new UserContext();

                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwt);

                    userId = token.Claims.Where(c => c.Type == "nameid")
                             .Select(c => Guid.Parse(c.Value)).FirstOrDefault();
                    if (userId == Guid.Empty)
                        return new UserContext();
                    else
                    {
                        claims = token.Claims;
                    }
                }

                try
                {
                    var userManager = c.GetService<UserManager<User>>();
                    var roleManager = c.GetService<RoleManager<Role>>();
                    //string userId = userManager.GetUserId(claimsPrincipal);
                    //id = Guid.Parse(userId);

                    var user = (userManager.FindByIdAsync(userId.ToString())).Result;

                    if (user == null)
                        return new UserContext();

                    var companyId = claims
                    .Where(c => c.Type == "CompanyId")
                    .Select(c => Guid.Parse(c.Value)).FirstOrDefault();

                    var companyName = claims
                    .Where(c => c.Type == "CompanyName")
                    .Select(c => c.Value).FirstOrDefault();

                    var roles = new List<string>(); // (userManager.GetRolesAsync(user).Result).ToList();
                    if (companyId != Guid.Empty || userId != Guid.Empty)
                    {
                        var roleService = c.GetRequiredService<SimpleRoleService>();
                        roles = roleService.GetUserRolesAsync(userId, companyId).Result;
                    }
                    var jtwFromHeaders = !string.IsNullOrEmpty(jwt) ? jwt : httpContextAccessor.HttpContext.Request.Headers["Authentication"].FirstOrDefault();
                    return new UserContext(userId, user.UserName, roles, companyId, companyName, jtwFromHeaders);
                }
                catch (Exception e)
                {
                    Logger.LogInformation($"Error occured, {e.Message}", e);
                };
                return new UserContext();
            });

            if (Environment.IsDevelopment())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagerService.Api", Version = "v1" });

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = @"JWT Authorization header using the Bearer scheme. Example:  Enter your token in the text input below. Example: '12345abcdef'",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",

                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                { Name = "Bearer",
                                  In = ParameterLocation.Header,
                                  Scheme = "oauth2",
                                  BearerFormat = "Bearer {token}",
                                  Reference = new OpenApiReference
                                  {
                                      Type = ReferenceType.SecurityScheme,
                                      Id = "Bearer"
                                  }
                                }, new string[] {}
                            }
                        }
                    );
                });
            }

            services.AddCors(options =>
            {
                options.AddPolicy("Policy",
                builder =>
                {
                    builder
                    //.WithOrigins("*")
                    .WithOrigins(protocols.CorsUrls.ToArray())
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagerService.Api v1"));
            }

            app.UseMiddleware<ExceptionMiddleWare>();
            app.UseMiddleware<UserMiddleWare>();
            app.Use(async (context, next) =>
            {
                var protocols = Configuration.GetSection("WebProtocolSettings").Get<WebProtocolSettings>();
                if (protocols.InstanceId == 0)
                    context.Response.Headers.Add("Service", "UMS");
                else
                    context.Response.Headers.Add("Service", $"UMS-{protocols.InstanceId}");
                await next.Invoke();
            });

            app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;

                if ((response.StatusCode == (int)HttpStatusCode.Unauthorized ||
             response.StatusCode == (int)HttpStatusCode.Forbidden) &&
             !context.HttpContext.Request.Path.StartsWithSegments("/api"))
                    response.Redirect("/Home/Login");

                //if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
                //    response.StatusCode == (int)HttpStatusCode.Forbidden)
                //{
                //    var message = "Unauthorized";
                //    response.Redirect($"/Home/Error?message={message}");
                //}


                //return Task.CompletedTask;
            });
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("Policy");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignalRHub>("/api/signalr");
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}