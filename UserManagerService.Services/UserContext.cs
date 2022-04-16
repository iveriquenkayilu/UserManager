using System.Collections.Generic;
using System.Linq;
using UserManagerService.Shared.Interfaces.Services;

namespace UserManagerService.Services
{
    public class UserContext : IUserContext
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }

        public UserContext()
        {
            //Roles = new List<string>();
        }

        public UserContext(long userId, string username)
        {
            UserId = userId;
            Username = username;
            //Roles = new List<string>();
        }

        public UserContext(long userId, string username, List<string> roles)
        {
            UserId = userId;
            Username = username;
            Roles = roles?.Select(r => r.ToUpper()).ToList();
        }

        public UserContext(long userId, string username, List<string> roles, long organizationId, string organizationName)
        {
            UserId = userId;
            Username = username;
            Roles = roles?.Select(r => r.ToUpper()).ToList();
            OrganizationId = organizationId;
            OrganizationName = organizationName;
        }
    }
}