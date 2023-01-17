using System;

namespace UserManagerService.Shared.Models.User
{
    public class AccessTokenModel
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}