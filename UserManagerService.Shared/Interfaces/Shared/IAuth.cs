using System.Collections.Generic;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Shared.Interfaces.Shared
{
    public interface IAuth
    {
        AuthTokenModel CreateSecurityToken(long userId, string username, List<string> roles, long orgId, string orgName);
        bool RevokeCachedRefreshToken(string refreshToken);
        RefreshTokenModel GetCachedRefreshTokenWithRequestIpValidation(string refreshToken);
    }
}
