using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class OrganizationUser : BaseEntity
    {
        public long OrganizationId { get; set; }
        public virtual IOrganization Organization { get; set; }
        public long UserId { get; set; }
        public virtual IUser User { get; set; }
    }
}