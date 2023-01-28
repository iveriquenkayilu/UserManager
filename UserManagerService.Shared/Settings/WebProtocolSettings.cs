using System.Collections.Generic;

namespace UserManagerService.Shared.Settings
{
    public class WebProtocolSettings
    {
        public string Urls { get; set; }

        public string Port { get; set; }
        public string FileServiceUrl { get; set; }
        public string EncryptionKey { get; set; }
        public int AccessTokenExpiresInMinutes { get; set; }
        public int RefreshTokenExpiresInMinutes { get; set; }
        public List<string> CorsUrls { get; set; }

        public int InstanceId { get; set; }
    }
}