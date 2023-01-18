using System;

namespace UserManagerService.Shared.Models.User
{
    public class LoginSessionModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Location { get; set; } // Lat Lng
        public string Status { get; set; }
        public string Device { get; set; }
        public string IpAddress { get; set; }
    }
}
