using System;

namespace UserManagerService.Shared.Models.User
{
    public class RefreshTokenInput
    {
        public string RefreshToken { get; set; }
		public Guid CompanyId { get; set; }
	}
}