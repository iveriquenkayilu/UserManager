namespace UserManagerService.Entities.Interfaces
{
    public interface IConctact : IBaseEntity
    {
        string Name { get; set; }
        string Value { get; set; }
        long ContactTypeId { get; set; }
        IContactType ContactType { get; set; }
    }
}
