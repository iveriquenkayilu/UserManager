namespace UserManagerService.Entities.Interfaces
{
    public interface IContactType : IBaseEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        string Format { get; set; }
    }
}
