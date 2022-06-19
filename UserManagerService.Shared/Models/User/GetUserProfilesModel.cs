using System;
using System.Collections.Generic;

namespace UserManagerService.Shared.Models.User
{
    public class GetUserProfilesModel
    {
        public List<Guid> UserIds { get; set; }
    }
}
