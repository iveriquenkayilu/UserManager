using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class Address : BaseEntity, IAddress
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Number { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
