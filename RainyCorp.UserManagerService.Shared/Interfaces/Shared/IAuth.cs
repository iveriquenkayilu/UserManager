using RainyCorp.UserManagerService.Entities;
using RainyCorp.UserManagerService.Shared.Models.User;
using System.Collections.Generic;

namespace RainyCorp.UserManagerService.Shared.Interfaces.Shared
{
    public interface IAuth
    {
        AuthTokenModel CreateSecurityToken(long userId, string username, List<string> roles);
        bool RevokeCachedRefreshToken(string refreshToken);
        RefreshTokenModel GetCachedRefreshTokenWithRequestIpValidation(string refreshToken);
    }
}
