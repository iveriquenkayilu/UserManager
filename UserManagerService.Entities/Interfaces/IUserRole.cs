namespace UserManagerService.Entities.Interfaces
{
    public interface IUserRole : IBaseEntity
    {
        long UserId { get; set; }
        long RoleId { get; set; }
        IRole Role { get; set; }
        IUser User { get; set; }
    }
}
