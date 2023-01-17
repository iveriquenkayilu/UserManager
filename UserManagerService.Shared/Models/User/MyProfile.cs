using System.Collections.Generic;
using UserManagerService.Shared.Models.Company;

namespace UserManagerService.Shared.Models.User
{
	public class MyProfile : UserProfile
	{
		public List<CompanyShortModel> Companies { get; set; }
	}
}
