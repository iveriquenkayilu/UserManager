using Microsoft.AspNetCore.Identity;
using UserManagerService.Entities.Interfaces;
using System;

namespace UserManagerService.Entities
{
    public class UserToken : IdentityUserToken<long>, IUserToken
    {
        public virtual IUser User { get; set; }
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
