using System;

namespace UserManagerService.Shared.Models.Service
{
    public class ServiceApiKeyModel
    {
        public string KeyName { get; set; }
        public string Value { get; set; }   // TODO hash +salt
        public DateTime ExpiresAt { get; set; }
    }
}