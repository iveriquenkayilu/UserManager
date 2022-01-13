using RainyCorp.UserManagerService.Shared.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;

namespace RainyCorp.UserManagerService.Services
{
    public class UserContext : IUserContext
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

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
    }
}