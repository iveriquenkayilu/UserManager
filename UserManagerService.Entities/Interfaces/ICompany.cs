namespace UserManagerService.Entities.Interfaces
{
    public interface ICompany : IBaseEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        long CompanyTypeId { get; set; }
        ICompanyType CompanyType { get; set; }
    }
}