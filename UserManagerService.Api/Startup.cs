using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OBS.UserManagementService.Domain.Helpers;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using UserManagerService.Api.MiddleWares;
using UserManagerService.Entities;
using UserManagerService.Interfaces.Repositories;
using UserManagerService.Repository;
using UserManagerService.Services;
using UserManagerService.Services.Interfaces;
using UserManagerService.Shared.Helpers;
using UserManagerService.Shared.Hubs;
using UserManagerService.Shared.Interfaces.Services;
using UserManagerService.Shared.Interfaces.Shared;
using UserManagerService.Shared.Settings;

namespace UserManagerService
{
    public class Startup
    {
        private readonly IWebHostEnvironment Environment;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            //if (Environment.IsDevelopment())
            //    services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), builder =>
            //    {
            //        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            //    }));
            //else
            {
                var connectionString = Configuration.GetConnectionString("MySqlConnection");
                services.AddDbContext<ApplicationDbContext>(options =>
                           options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23)),
                           options => options.EnableRetryOnFailure()));
            }

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
             .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddControllers();

            //register the initializer
            services.AddAsyncInitializer<Initializer>();

            services.AddSignalR(options =>
            {
                //options.KeepAliveInterval = TimeSpan.FromDays(30);
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(1);
            });
            // registers Repository services

            //services.AddScoped<IHttpOrchestrator, HttpOrchestrator>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            services.AddScoped<IApiService, ApiService>();

            // registers AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddScoped<RoleManager<Role>>();
            services.AddScoped<UserManager<User>>();
            services.AddScoped<SignInManager<User>>();

            services.Configure<WebProtocolSettings>(Configuration.GetSection("WebProtocolSettings"));
            var protocols = Configuration.GetSection("WebProtocolSettings").Get<WebProtocolSettings>();

            services.AddSingleton<IAuth, Auth>();

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
            });

            // check between scoped and transcient
            services.AddTransient<IUserContext, UserContext>(c =>
            {
                //TODO Handle incorrect cases.
                IHttpContextAccessor httpContextAccessor = c.GetService<IHttpContextAccessor>();

                ClaimsPrincipal claimsPrincipal = httpContextAccessor.HttpContext?.User;

                if (claimsPrincipal == null)
                    return new UserContext();

                var userManager = c.GetService<UserManager<User>>();
                var roleManager = c.GetService<RoleManager<Role>>();
                string userId = userManager.GetUserId(claimsPrincipal);
                long id = 0;

                if (!long.TryParse(userId, out id))
                    return new UserContext();

                var user = (userManager.FindByIdAsync(id.ToString())).Result;
                var roles = (userManager.GetRolesAsync(user).Result).ToList();

                var organizationId = ((ClaimsIdentity)claimsPrincipal.Identity).Claims
                .Where(c => c.Type == "CompanyId")
                .Select(c => Int64.Parse(c.Value)).FirstOrDefault();

                var organizationName = ((ClaimsIdentity)claimsPrincipal.Identity).Claims
                .Where(c => c.Type == "CompanyName")
                .Select(c => c.Value).FirstOrDefault();

                return new UserContext(id, user.UserName, roles, organizationId, organizationName);
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
              //TODO remove this on Prod
                services.AddCors(options =>
                {
                    options.AddPolicy("Policy",
                    builder =>
                    {
                        builder
                        //.WithOrigins("*")
                       .WithOrigins(protocols.CorsUrls.ToArray()) // get urls from appsettings
                      //.AllowCredentials()
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
                app.UseCors("Policy"); // TODO remove this. It is for dev

            app.UseMiddleware<ExceptionMiddleWare>();
            app.UseMiddleware<UserMiddleWare>();
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Service", "UMS");
                await next.Invoke();
            });

            app.UseHttpsRedirection();

            //app.UseStaticFiles();

            app.UseRouting();
            //app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignalRHub>("/api/signalr");
                endpoints.MapControllers();
            });
        }
    }
}