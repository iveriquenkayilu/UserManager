using System;
using System.Collections.Generic;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Shared.Models.Company
{
    public class CompanyModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<UserModel> Users { get; set; }
    }
}
