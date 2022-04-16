namespace UserManagerService.Shared.Models.User
{
    /// <summary>
    /// Implements the register model.
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        public long OrganizationId { get; set; }
    }
}