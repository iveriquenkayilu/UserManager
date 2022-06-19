using System;

namespace UserManagerService.Entities.Interfaces
{
    public interface IUserRole : IBaseEntity
    {
        Guid UserId { get; set; }
        Guid RoleId { get; set; }
        IRole Role { get; set; }
        IUser User { get; set; }
    }
}
