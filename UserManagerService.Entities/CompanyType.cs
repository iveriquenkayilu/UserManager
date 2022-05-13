using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class CompanyType : BaseEntity, ICompanyType
    {
        public string Name { get; set; }
    }
}
