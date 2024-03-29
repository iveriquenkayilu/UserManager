﻿using System;

namespace UserManagerService.Entities
{
    public class TeamUser : BaseCompanyEntity
	{
        public Guid TeamId { get; set; }
        public virtual Team Team { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}