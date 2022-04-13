using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class Organization : BaseEntity, IOrganization
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long OrganizationTypeId { get; set; }
        public virtual IOrganizationType OrganizationType { get; set; }
    }
}