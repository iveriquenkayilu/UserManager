namespace UserManagerService.Entities.Interfaces
{
    public interface IOrganization : IBaseEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        long OrganizationTypeId { get; set; }
        IOrganizationType OrganizationType { get; set; }
    }
}