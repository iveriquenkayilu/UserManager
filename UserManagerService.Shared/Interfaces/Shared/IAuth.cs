using System;
using System.Collections.Generic;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Shared.Interfaces.Shared
{
    public interface IAuth
    {
        AuthTokenModel CreateSecurityToken(Guid userId, string username, List<string> roles, Guid orgId, string orgName);
        bool RevokeCachedRefreshToken(string refreshToken);
        RefreshTokenModel GetCachedRefreshTokenWithRequestIpValidation(string refreshToken);
    }
}
