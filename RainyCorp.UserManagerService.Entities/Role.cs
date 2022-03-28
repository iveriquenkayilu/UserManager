using Microsoft.AspNetCore.Identity;
using RainyCorp.UserManagerService.Entities.Interfaces;
using System;

namespace RainyCorp.UserManagerService.Entities
{
    public class Role : IdentityRole<long>, IRole
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}