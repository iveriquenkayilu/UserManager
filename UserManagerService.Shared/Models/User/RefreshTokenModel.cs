using System;

namespace UserManagerService.Shared.Models.User
{
    public class RefreshTokenModel
    {
        public Guid UserId { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string IpAddress { get; set; }
    }
}
