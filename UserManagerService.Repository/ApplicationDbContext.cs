using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using UserManagerService.Entities;

namespace UserManagerService.Repository
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, UserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyUser> CompanyUsers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamUser> TeamUsers { get; set; }
        public DbSet<LoginSession> LoginSessions { get; set; }

        //public DbSet<ServiceApiKey> ServiceApiKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.UseCollation("SQL_Latin1_General_CP1_CI_AI");

            builder.Entity<Company>(c =>{
                c.HasIndex(s => s.Name);
                c.Property(p=>p.Type).HasConversion(typeof(string))
                 .HasMaxLength(50);
                //.HasConversion(new EnumToStringConverter<MyEnumType>());
            });
           

            builder.Entity<User>(u =>
            {
                u.HasIndex(i => i.Email).IsUnique();
                u.HasIndex(i => i.NormalizedEmail).IsUnique();
            });

            builder.Entity<UserRole>(ur =>
            {
                ur.Ignore(u => u.Id);

                ur.HasOne(r => r.User)
                .WithMany()
              //.WithMany(r => (List<UserRole>)r.UserRoles)
              .HasForeignKey(r => r.UserId);

                ur.HasOne(r => r.Role)
                    .WithMany()
                    .HasForeignKey(r => r.RoleId);
            });

            builder.Entity<UserToken>(uk =>
            {
                uk.Ignore(u => u.Id)
                .Ignore(u => u.LoginProvider).Ignore(u => u.Name);

                uk.HasKey(u => new { u.UserId, u.Value });

                uk.HasOne(u => u.User).WithMany().HasForeignKey(u => u.UserId);
            });

            builder.Entity<CompanyUser>(o =>
            {
                o.HasIndex(u => new { u.CompanyId, u.UserId }).IsUnique();

                o.HasOne(u => u.User).WithMany(u => u.CompanyUsers).HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                o.HasOne(u => u.Company).WithMany(c => c.CompanyUsers).HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Contact>(c =>
            {
                c.HasOne(u => u.ContactType).WithMany().HasForeignKey(u => u.ContactTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<TeamUser>(c =>
            {
                c.HasOne(u => u.Team).WithMany().HasForeignKey(u => u.
                TeamId)
                .OnDelete(DeleteBehavior.Restrict);

                c.HasOne(u => u.User).WithMany().HasForeignKey(u => u.
                 UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}