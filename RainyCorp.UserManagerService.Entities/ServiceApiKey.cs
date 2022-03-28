using RainyCorp.UserManagerService.Entities.Interfaces;
using System;

namespace RainyCorp.UserManagerService.Entities
{
    /// <summary>
    /// Implements the anonymous user entity.
    /// </summary>
    public class ServiceApiKey : BaseEntity, IServiceApiKey  // for micro services
    {
        public string KeyName { get; set; }
        public string Value { get; set; }   // TODO hash +salt
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(5);
        public string AddressIp { get; set; }
        public string Port { get; set; }
        public string Details { get; set; }
    }
}