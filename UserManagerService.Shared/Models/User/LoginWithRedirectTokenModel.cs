using System;

namespace UserManagerService.Shared.Models.User
{
    public class LoginWithRedirectTokenModel
    {
        public Guid SessionId { get; set; }
        public string ReturnUrl { get; set; }
    }
}
