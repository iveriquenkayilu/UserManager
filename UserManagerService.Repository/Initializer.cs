using AspNetCore.AsyncInitialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagerService.Entities;
using UserManagerService.Shared.Constants;

namespace UserManagerService.Repository
{
    public class Initializer : IAsyncInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<Initializer> _logger;
        private readonly IHostEnvironment _environment;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public Initializer(ApplicationDbContext dbContext, ILogger<Initializer> logger, UserManager<User> userManager, RoleManager<Role> roleManager, IHostEnvironment environment)
        {
            _dbContext = dbContext;
            _logger = logger;
            _environment = environment;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation($"Hosting environment: {_environment.EnvironmentName}");
                _logger.LogInformation("Initializing the database and applying migrations");

                if (_environment.IsDevelopment()) // Inverse
                    await _dbContext.Database.EnsureCreatedAsync();
                else
                    await _dbContext.Database.MigrateAsync();

                await AddDefaultValuesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
        }

        private async Task AddDefaultValuesAsync()
        {
            if (!await _dbContext.Roles.AnyAsync())
            {
                await _roleManager.CreateAsync(new Role { Name = Roles.ADMIN, CreatedAt = DateTime.Now });
                await _roleManager.CreateAsync(new Role { Name = "User", CreatedAt = DateTime.Now });
            }

            if (!await _dbContext.Users.AnyAsync())
            {
                var roles = await _dbContext.Roles.Select(r => r.Id).ToListAsync();
                await CreateUser(roles, Admin.FirstName, Admin.LastName, Admin.Email, Admin.UserName, Admin.DefaultPassword);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CreateUser(List<long> roles, string firstName, string lastName, string email, string userName,
        string password)
        {
            _logger.LogInformation($"Create user with email `{email}` for application");
            var user = new User
            {
                Email = email,
                NormalizedEmail = email.ToUpper(),
                UserName = userName,
                NormalizedUserName = email.ToUpper(),
                AccessFailedCount = 0,
                EmailConfirmed = true,
                Name = firstName,
                Surname = lastName,
                LockoutEnabled = false,
                CreatedAt = DateTime.Now
            };

            var ir = await _userManager.CreateAsync(user, password);
            var userId = await _dbContext.Users.Where(u => u.UserName.ToUpper() == userName.ToUpper()).Select(u => u.Id).FirstOrDefaultAsync();

            if (ir.Succeeded)
            {
                var userRoles = new List<UserRole>();
                roles.ForEach(r => { userRoles.Add(new UserRole { RoleId = r, UserId = userId }); });
                await _dbContext.UserRoles.AddRangeAsync(userRoles);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Created user `{userName}` successfully");

                await CreateAdminCompanyAsync(userId);
            }
            else
                _logger.LogError("Failed to create user");
        }


        private async Task CreateAdminCompanyAsync(long userId)
        {

            if (!await _dbContext.OrganizationTypes.AnyAsync())
            {
                var type = new OrganizationType { Name = Admin.OrganizationType };
                await _dbContext.AddAsync(type);
                await _dbContext.SaveChangesAsync();
            }

            if (!await _dbContext.Organizations.AnyAsync())
            {
                var typeId = (await _dbContext.OrganizationTypes.Where(o => o.Name == Admin.OrganizationName).SingleOrDefaultAsync()).Id;
                var organization = new Organization { Name = Admin.OrganizationName, OrganizationTypeId = typeId };
                await _dbContext.AddAsync(organization);
                await _dbContext.SaveChangesAsync();

                if (!await _dbContext.OrganizationUsers.AnyAsync())
                {

                    var organizationUser = new OrganizationUser { OrganizationId = organization.Id, UserId = userId };
                    await _dbContext.AddAsync(organizationUser);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
