using System;

namespace UserManagerService.Entities
{
    public class CompanyUser : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}