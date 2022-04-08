using UserManagerService.Entities;
using UserManagerService.Shared.Models.User;
using System.Collections.Generic;

namespace UserManagerService.Shared.Interfaces.Shared
{
    public interface IAuth
    {
        AuthTokenModel CreateSecurityToken(long userId, string username, List<string> roles);
        bool RevokeCachedRefreshToken(string refreshToken);
        RefreshTokenModel GetCachedRefreshTokenWithRequestIpValidation(string refreshToken);
    }
}
