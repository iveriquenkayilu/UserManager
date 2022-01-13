﻿using System;

namespace RainyCorp.UserManagerService.Shared.Models.User
{
    public class RefreshTokenModel
    {
        public long UserId { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string IpAddress { get; set; }
    }
}
