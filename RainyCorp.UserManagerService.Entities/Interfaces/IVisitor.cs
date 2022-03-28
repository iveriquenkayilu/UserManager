using System.Collections.Generic;

namespace RainyCorp.UserManagerService.Entities.Interfaces
{
    /// <summary>
    /// Defines the anonymous user entity.
    /// </summary>
    public interface IVisitor : IBaseEntity
    {

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the instagram.
        /// </summary>
        string Instagram { get; set; }


        /// <summary>
        /// Gets or sest the address Ip
        /// </summary>
        public string AddressIp { get; set; }

        /// <summary>
        /// Gets or sets the operating system.
        /// </summary>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Gets or sets the access type.
        /// </summary>
        public string AccessType { get; set; }
    }
}
