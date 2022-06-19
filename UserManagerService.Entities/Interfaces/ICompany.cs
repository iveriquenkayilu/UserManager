using System;
using System.Collections.Generic;

namespace UserManagerService.Entities.Interfaces
{
    public interface ICompany : IBaseEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        Guid CompanyTypeId { get; set; }
        ICompanyType CompanyType { get; set; }
        List<CompanyUser> CompanyUsers { get; set; }
    }
}