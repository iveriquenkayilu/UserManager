using System;
using System.Collections.Generic;

namespace UserManagerService.Entities.Interfaces
{
    /// <summary>
    /// Defines the anonymous user entity.
    /// </summary>
    public interface IServiceApiKey : IBaseEntity
    {
         string KeyName { get; set; }
         string Value { get; set; }
         string AddressIp { get; set; }

         string Port { get; set; }

         string Details { get; set; }
        DateTime ExpiresAt { get; set; }
    }
}
