using System;

namespace UserManagerService.Entities.Interfaces
{
    public interface IUserToken : IBaseEntity
    {
        Guid UserId { get; set; }
        IUser User { get; set; }
        DateTime ExpiredAt { get; set; }
    }
}
