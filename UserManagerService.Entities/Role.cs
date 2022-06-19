using Microsoft.AspNetCore.Identity;
using System;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class Role : IdentityRole<Guid>, IBaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid CreatorId { get; set; }
    }
}