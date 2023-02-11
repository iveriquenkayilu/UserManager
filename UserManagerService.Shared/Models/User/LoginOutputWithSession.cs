using System;

namespace UserManagerService.Shared.Models.User
{
    public class LoginOutputWithSession : UserProfile
    {
        public Guid SessionId { get; set; }
    }
}
