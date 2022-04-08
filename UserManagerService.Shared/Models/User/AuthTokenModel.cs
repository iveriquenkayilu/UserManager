namespace UserManagerService.Shared.Models.User
{
    public class AuthTokenModel
    {
        public int Duration { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}