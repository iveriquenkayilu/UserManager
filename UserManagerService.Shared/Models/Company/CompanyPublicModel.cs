using System;
using UserManagerService.Entities.Datatypes;

namespace UserManagerService.Shared.Models.Company
{
    public class CompanyPublicModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CompanyTypeOption Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
