using System;
using System.ComponentModel;

namespace UserManagerService.Shared.Models.User
{
    public class LoginToCompanyInputModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [DefaultValue(null)]
        public Guid? CompanyId { get; set; }
    }
}