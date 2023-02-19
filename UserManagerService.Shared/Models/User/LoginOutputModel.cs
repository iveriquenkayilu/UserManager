using System;
using System.Collections.Generic;
using UserManagerService.Shared.Models.Company;

namespace UserManagerService.Shared.Models.User
{
	public class LoginOutputModel
	{
		public List<CompanyShortModel> Companies { get; set; }
		public AuthTokenModel AuthTokens { get; set; }
		public Guid SessionId { get; set; }
	}
}
