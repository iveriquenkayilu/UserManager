using System;
using System.Collections.Generic;

namespace UserManagerService.Shared.Models.Roles
{
    public class UserRoleInputModel
    {
        public List<Guid> RoleIds { get; set; }
        public Guid UserId { get; set; }
    }
}
