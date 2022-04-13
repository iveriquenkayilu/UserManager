using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class TeamUser : BaseEntity, ITeamUser
    {
        public long TeamId { get; set; }
        public virtual ITeam Team { get; set; }
        public long UserId { get; set; }
        public virtual IUser User { get; set; }
    }
}