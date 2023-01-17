using Microsoft.AspNetCore.Identity;
using System;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
	public class UserRole : IdentityUserRole<Guid>, IBaseCompanyEntity
	{
		public Guid Id { get; set; }
		public virtual User User { get; set; }
		public virtual Role Role { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }
		public Guid CreatorId { get; set; }
		public Guid CompanyId { get; set; }
	}
}