using System.Collections.Generic;

namespace UserManagerService.Entities.Interfaces
{
    public interface IUser : IBaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        bool IsConnected { get; set; }
        public string UserName { get; set; }
        string Picture { get; set; }
        List<CompanyUser> CompanyUsers { get; set; }
    }
}