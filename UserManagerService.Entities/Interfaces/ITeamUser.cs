namespace UserManagerService.Entities.Interfaces
{
    public interface ITeamUser : IBaseEntity
    {
        long TeamId { get; set; }
        ITeam Team { get; set; }
        long UserId { get; set; }
        IUser User { get; set; }
    }
}
