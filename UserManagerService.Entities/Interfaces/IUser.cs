using System;
using System.Collections.Generic;

namespace UserManagerService.Entities.Interfaces
{
    /// <summary>
    /// Defines the user entities
    /// </summary>
    public interface IUser : IBaseEntity
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
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; set; }

        public string UserName { get; set; }
        string Picture { get; set; }
        List<CompanyUser> CompanyUsers { get; set; }
    }
}