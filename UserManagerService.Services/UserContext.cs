using System;
using System.Collections.Generic;
using System.Linq;
using UserManagerService.Shared.Interfaces.Services;

namespace UserManagerService.Services
{
    public class UserContext : IUserContext
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }

        public UserContext()
        {
            //Roles = new List<string>();
        }

        public UserContext(Guid userId, string username)
        {
            UserId = userId;
            Username = username;
            //Roles = new List<string>();
        }

        public UserContext(Guid userId, string username, List<string> roles)
        {
            UserId = userId;
            Username = username;
            Roles = roles?.Select(r => r.ToUpper()).ToList();
        }

        public UserContext(Guid userId, string username, List<string> roles, Guid companyId, string companyName)
        {
            UserId = userId;
            Username = username;
            Roles = roles?.Select(r => r.ToUpper()).ToList();
            CompanyId = companyId;
            CompanyName = companyName;
        }
    }
}