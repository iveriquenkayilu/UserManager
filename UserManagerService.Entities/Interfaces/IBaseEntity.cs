﻿using System;

namespace UserManagerService.Entities.Interfaces
{
    public interface IBaseEntity
    {
        long Id { get; set; }

        DateTime CreatedAt { get; set; }

        DateTime UpdatedAt { get; set; }

        DateTime? DeletedAt { get; set; }
        long CreatorId { get; set; }
    }
}