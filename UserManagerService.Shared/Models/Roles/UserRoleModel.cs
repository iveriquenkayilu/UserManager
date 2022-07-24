using System;

namespace UserManagerService.Shared.Models.Roles
{
    public class UserRoleModel
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
    }
}
