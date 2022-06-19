using System;

namespace UserManagerService.Shared.Models.Company
{
    public class CompanyUserInputModel
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}
