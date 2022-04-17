using System.Collections.Generic;

namespace UserManagerService.Shared.Models.User
{
    public class GetUserProfilesModel
    {
        public List<long> UserIds { get; set; }
    }
}
