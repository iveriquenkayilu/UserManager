using System;

namespace RainyCorp.UserManagerService.Entities.Interfaces
{
    public interface IUserToken : IBaseEntity
    {
        long UserId { get; set; }
        IUser User { get; set; }
        DateTime ExpiredAt { get; set; }
    }
}
