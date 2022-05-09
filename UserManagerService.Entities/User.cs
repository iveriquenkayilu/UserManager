using Microsoft.AspNetCore.Identity;
using System;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    /// <summary>
    /// Implements the user entity
    /// </summary>
    public class User : IdentityUser<long>, IUser
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the Last update date.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; set; } = false;

        public long CreatorId { get; set; }

        public string Picture { get; set; }
    }
}