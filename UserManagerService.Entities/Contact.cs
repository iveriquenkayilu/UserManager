using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class Contact : BaseEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public long ContactTypeId { get; set; }
        public virtual IContactType ContactType { get; set; }
    }
}
