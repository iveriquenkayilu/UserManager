using System.Collections.Generic;

namespace UserManagerService.Shared.Interfaces.Services
{
    public interface IUserContext
    {
        long UserId { get; set; }
        string Username { get; set; }

        List<string> Roles { get; set; }
        long OrganizationId { get; set; }
        string OrganizationName { get; set; }
    }
}