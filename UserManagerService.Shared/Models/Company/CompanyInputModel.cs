using UserManagerService.Entities.Datatypes;

namespace UserManagerService.Shared.Models.Company
{
    public class CompanyInputModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CompanyTypeOption Type { get; set; }
    }
}
