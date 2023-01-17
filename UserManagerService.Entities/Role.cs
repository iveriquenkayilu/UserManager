using Microsoft.AspNetCore.Identity;
using System;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
	public class Role : IdentityRole<Guid>, IBaseCompanyEntity
	{
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }
		public Guid CreatorId { get; set; }
		public Guid CompanyId { get; set; }
	}
}