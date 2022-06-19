using System.Collections.Generic;

namespace UserManagerService.Entities.Interfaces
{
    public interface IVisitor : IBaseEntity
    {
        string Name { get; set; }

        string Email { get; set; }
        string PhoneNumber { get; set; }

        string Instagram { get; set; }

        public string AddressIp { get; set; }

        public string OperatingSystem { get; set; }
     
        public string Device { get; set; }

        public string AccessType { get; set; }
    }
}
