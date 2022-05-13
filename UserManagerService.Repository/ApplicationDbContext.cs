﻿using Microsoft.AspNetCore.Identity;
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
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
        public DbSet<CompanyUser> CompanyUsers { get; set; }
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

            builder.Entity<Company>(o =>
            {
                o.HasOne(u => (CompanyType)u.CompanyType).WithMany().HasForeignKey(u => u.CompanyTypeId)
                                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<CompanyUser>(o =>
            {
                o.HasIndex(u => new { u.CompanyId, u.UserId }).IsUnique();

                o.HasOne(u => (User)u.User).WithMany().HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                o.HasOne(u => (Company)u.Company).WithMany().HasForeignKey(u => u.CompanyId)
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