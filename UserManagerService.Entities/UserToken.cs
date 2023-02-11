using Microsoft.AspNetCore.Identity;
using System;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
	public class UserToken : IdentityUserToken<Guid>, IBaseCompanyEntity
	{
		public Guid Id { get; set; }
        public virtual User User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
		public DateTime? DeletedAt { get; set; }
		public DateTime ExpiredAt { get; set; }
		public Guid CreatorId { get; set; }
		public Guid CompanyId { get; set; }
	}
}
