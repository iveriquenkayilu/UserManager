using System;

namespace UserManagerService.Entities.Interfaces
{
	public interface IBaseCompanyEntity : IBaseEntity
	{
		Guid CompanyId { get; set; }
	}
}
