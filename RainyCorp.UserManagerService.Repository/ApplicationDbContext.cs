using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RainyCorp.UserManagerService.Entities;

namespace RainyCorp.UserManagerService.Repository
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, UserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<ServiceApiKey> ServiceApiKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ServiceApiKey>().HasIndex(s => s.KeyName).IsUnique();
            //builder.Entity<Role>().Ignore(r => r.CreatedAt);

            builder.Entity<UserRole>(ur =>
            {
                ur.Ignore(u => u.Id);

                ur.HasOne(r => (User)r.User)
                .WithMany()
              //.WithMany(r => (List<UserRole>)r.UserRoles)
              .HasForeignKey(r => r.UserId);

                ur.HasOne(r => (Role)r.Role)
                    .WithMany()
                    .HasForeignKey(r => r.RoleId);
            });

            builder.Entity<UserToken>(uk =>
            {
                uk.Ignore(u => u.Id)
                .Ignore(u => u.LoginProvider).Ignore(u => u.Name);

                uk.HasKey(u => new { u.UserId, u.Value });

                uk.HasOne(u => (User)u.User).WithMany().HasForeignKey(u => u.UserId);
            });
        }
    }
}