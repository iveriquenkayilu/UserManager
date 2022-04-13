using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class Team : BaseEntity, ITeam
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}