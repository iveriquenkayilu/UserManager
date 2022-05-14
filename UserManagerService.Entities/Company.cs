using System.Collections.Generic;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class Company : BaseEntity, ICompany
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long CompanyTypeId { get; set; }
        public virtual ICompanyType CompanyType { get; set; }
        public List<CompanyUser> CompanyUsers { get; set; }
    }
}