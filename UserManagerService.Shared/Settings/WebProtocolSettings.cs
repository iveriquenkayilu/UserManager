namespace UserManagerService.Shared.Settings
{
    /// <summary>
    /// Implements the web protocol settings.
    /// </summary>
    public class WebProtocolSettings
    {
        public string Urls { get; set; }

        public string Port { get; set; }
        public string ResetPasswordBaseUrl { get; set; }
        public string EncryptionKey { get; set; }
        public int AccessTokenExpiresInMinutes { get; set; }
        public int RefreshTokenExpiresInMinutes { get; set; }
    }
}