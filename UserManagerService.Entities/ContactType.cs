using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class ContactType : BaseEntity, IContactType
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
    }
}
