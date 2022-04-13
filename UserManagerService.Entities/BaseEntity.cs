﻿using System;
using System.ComponentModel.DataAnnotations;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    /// <summary>
    /// Implements the base entity.
    /// </summary>
    public class BaseEntity : IBaseEntity
    {
        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        [Key]
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }
        public long CreatorId { get; set; }
    }
}