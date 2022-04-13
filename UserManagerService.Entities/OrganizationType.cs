using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class OrganizationType : BaseEntity, IOrganizationType
    {
        public string Name { get; set; }
    }
}
