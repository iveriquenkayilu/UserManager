using System;

namespace UserManagerService.Shared.Models.User
{
    public class LoginWithRedirectOutputTokenModel
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
    }
}