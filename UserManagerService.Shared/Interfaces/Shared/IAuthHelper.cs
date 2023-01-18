using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagerService.Shared.Models.Company;
using UserManagerService.Shared.Models.Helpers;
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
        AccessTokenModel GetAccessToken(Guid userId, string username, List<string> roles, CompanyShortModel company);
        Task<GetLocationModel> GetIpAddressLocation(string ip);
    }
}
