using System;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
	public class BaseCompanyEntity : BaseEntity, IBaseCompanyEntity
	{
		public Guid CompanyId { get; set; }
	}
}
