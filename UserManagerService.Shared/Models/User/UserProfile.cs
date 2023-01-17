using System;

namespace UserManagerService.Shared.Models.User
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Picture { get; set; }
    }
}
