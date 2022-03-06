using System;

namespace RainyCorp.UserManagerService.Shared.Models.User
{
    public class UserModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsConnected { get; set; }
    }
}