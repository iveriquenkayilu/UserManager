using System;
using System.ComponentModel.DataAnnotations;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Entities
{
    public class BaseEntity : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }
        public Guid CreatorId { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}