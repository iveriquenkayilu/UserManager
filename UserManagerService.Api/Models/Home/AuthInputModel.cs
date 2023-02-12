using System;

namespace UserManagerService.Models
{
    public class AuthInputModel
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public Guid? CompanyId { get; set; }
        public string ReturnUrl { get; set; }
    }
}
