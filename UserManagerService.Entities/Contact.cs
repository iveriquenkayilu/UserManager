using System;

namespace UserManagerService.Entities
{
    public class Contact : BaseEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid ContactTypeId { get; set; }
        public virtual ContactType ContactType { get; set; }
    }
}
