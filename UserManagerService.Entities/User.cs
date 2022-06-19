using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class User : IdentityUser<Guid>, IBaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsConnected { get; set; } = false;
        public Guid CreatorId { get; set; }
        public string Picture { get; set; }
        public List<CompanyUser> CompanyUsers { get; set; }
    }
}