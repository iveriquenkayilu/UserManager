using System;
using System.Collections.Generic;

namespace UserManagerService.Shared.Interfaces.Services
{
    public interface IUserContext
    {
        Guid UserId { get; set; }
        string Username { get; set; }
        List<string> Roles { get; set; }
        Guid CompanyId { get; set; }
        string CompanyName { get; set; }
        string JWTToken { get; set; }

        List<string> GetRoles();
        bool IsUserAdmin();
    }
}