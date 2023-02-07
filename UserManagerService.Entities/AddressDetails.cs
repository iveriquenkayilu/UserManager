using System;

namespace UserManagerService.Entities
{
    public class AddressDetails : BaseEntity
    {
        public string Description { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Number { get; set; }
        public Guid AddressId { get; set; }
        public virtual Address Address { get; set; }
    }
}
