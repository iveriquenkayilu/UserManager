using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagerService.Entities;

namespace UserManagerService.Repository
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, UserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationType> OrganizationTypes { get; set; }
        public DbSet<OrganizationUser> OrganizationUsers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamUser> TeamUsers { get; set; }

        //public DbSet<ServiceApiKey> ServiceApiKeys { get; set; }

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

            builder.Entity<Organization>(o =>
            {
                o.HasOne(u => (OrganizationType)u.OrganizationType).WithMany().HasForeignKey(u => u.OrganizationTypeId)
                                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<OrganizationUser>(o =>
            {
                o.HasIndex(u => new { u.OrganizationId, u.UserId }).IsUnique();

                o.HasOne(u => (User)u.User).WithMany().HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                o.HasOne(u => (Organization)u.Organization).WithMany().HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Contact>(c =>
            {

                c.HasOne(u => (ContactType)u.ContactType).WithMany().HasForeignKey(u => u.ContactTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<TeamUser>(c =>
            {
                c.HasOne(u => (Team)u.Team).WithMany().HasForeignKey(u => u.
                TeamId)
                .OnDelete(DeleteBehavior.Restrict);

                c.HasOne(u => (User)u.User).WithMany().HasForeignKey(u => u.
                UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}