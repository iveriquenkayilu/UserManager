namespace UserManagerService.Shared.Models.User
{
    public class UserProfile
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }

        public string Surname { get; set; }
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
    }
}
