namespace UserManagerService.Shared.Models.User
{
    public class LoginInputModel
    {
        public string Email { get; set; } // Phone Number
        public string Username { get; set; }
        public string Password { get; set; }
    }
}