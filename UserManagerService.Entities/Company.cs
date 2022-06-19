using System;
using System.Collections.Generic;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CompanyTypeId { get; set; }
        public virtual CompanyType CompanyType { get; set; }
        public List<CompanyUser> CompanyUsers { get; set; }
    }
}