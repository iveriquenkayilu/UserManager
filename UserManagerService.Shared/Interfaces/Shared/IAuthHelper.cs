using System;
using System.Collections.Generic;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Shared.Interfaces.Shared
{
    public interface IAuthHelper
    {
        AuthTokenModel CreateSecurityToken(Guid userId, string username, List<string> roles, CompanyShortModel company);
        bool RevokeCachedRefreshToken(string refreshToken);
        RefreshTokenModel GetCachedRefreshTokenWithRequestIpValidation(string refreshToken);
        string GetRequestIPAddress();
        VisitorModel GetVisitorInfo();
    }
}
