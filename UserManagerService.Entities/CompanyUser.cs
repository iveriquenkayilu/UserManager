using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class CompanyUser : BaseEntity
    {
        public long CompanyId { get; set; }
        public virtual ICompany Company { get; set; }
        public long UserId { get; set; }
        public virtual IUser User { get; set; }
    }
}