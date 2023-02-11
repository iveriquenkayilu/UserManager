using System;

namespace UserManagerService.Shared.Models.User
{
    public class LoginInputWithSession
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
