namespace UserManagerService.Entities.Interfaces
{
    public interface ITeam : IBaseEntity
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
