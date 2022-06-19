namespace UserManagerService.Entities
{
    public class Visitor : BaseEntity
    {

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Instagram { get; set; }

        public string AddressIp { get; set; }

        public string OperatingSystem { get; set; }

        public string Device { get; set; }

        public string AccessType { get; set; }
    }
}