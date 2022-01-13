using RainyCorp.UserManagerService.Entities;
using RainyCorp.UserManagerService.Shared.Models.User;
using System.Collections.Generic;

namespace RainyCorp.UserManagerService.Shared.Interfaces.Shared
{
    public interface IAuth
    {
        AuthenticationTokens CreateSecurityToken(long userId, string username, List<Role> roles);
        bool RevokeCachedRefreshToken(string refreshToken);
        RefreshTokenModel GetCachedRefreshTokenWithRequestIpValidation(string refreshToken);
    }
}
