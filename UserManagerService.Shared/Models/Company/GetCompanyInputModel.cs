namespace UserManagerService.Shared.Models.Company
{
    public class GetCompanyInputModel
    {
        public int Offset { get; set; }
        public int Limit { get; set; } = 10;
        public string Key { get; set; }
    }
}
