using System;

namespace UserManagerService.Shared.Models.User
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid? CompanyId { get; set; }
    }
}