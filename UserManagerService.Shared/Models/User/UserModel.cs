using System;

namespace UserManagerService.Shared.Models.User
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsConnected { get; set; }
        public string Picture { get; set; }
    }
}