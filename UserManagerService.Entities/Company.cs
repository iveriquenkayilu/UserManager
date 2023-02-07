using System;
using System.Collections.Generic;
using UserManagerService.Entities.Datatypes;

namespace UserManagerService.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CompanyTypeOption Type { get; set; }
        public string Logo { get; set; } // Url
        public Guid AddressId { get; set; }
        public virtual Address Address { get; set; }
        public List<CompanyUser> CompanyUsers { get; set; }
    }
}