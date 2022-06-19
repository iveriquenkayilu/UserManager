﻿using System;

namespace UserManagerService.Shared.Models.User
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Picture { get; set; }
    }
}
