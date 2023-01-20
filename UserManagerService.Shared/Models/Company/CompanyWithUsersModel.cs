using System;
using System.Collections.Generic;
using UserManagerService.Entities.Datatypes;
using UserManagerService.Shared.Models.User;

namespace UserManagerService.Shared.Models.Company
{
    public class CompanyWithUsersModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CompanyTypeOption Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<UserModel> Users { get; set; }
    }
}
