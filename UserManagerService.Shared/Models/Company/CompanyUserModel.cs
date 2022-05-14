namespace UserManagerService.Shared.Models.Company
{
    public class CompanyUserModel
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
    }
}
