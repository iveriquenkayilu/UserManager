using System;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
	public class CompanyUser : BaseCompanyEntity
	{
		public virtual Company Company { get; set; }
		public Guid UserId { get; set; }
		public virtual User User { get; set; }
	}
}