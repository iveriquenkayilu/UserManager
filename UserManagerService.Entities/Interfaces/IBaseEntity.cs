using System;

namespace UserManagerService.Entities.Interfaces
{
    public interface IBaseEntity
    {
        Guid Id { get; set; }

        DateTime CreatedAt { get; set; }

        DateTime UpdatedAt { get; set; }

        DateTime? DeletedAt { get; set; }
        Guid CreatorId { get; set; }
    }
}