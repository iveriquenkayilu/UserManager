using Microsoft.AspNetCore.Identity;
using System;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class UserRole : IdentityUserRole<long>, IUserRole
    {
        public virtual IUser User { get; set; }
        public virtual IRole Role { get; set; }
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long CreatorId { get; set; }
    }
}