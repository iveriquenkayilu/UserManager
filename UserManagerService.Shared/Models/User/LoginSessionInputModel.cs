using System;

namespace UserManagerService.Shared.Models.User
{
    public class LoginSessionInputModel
    {
        public DateTime? StartDate { get; set; } = DateTime.Now.AddDays(-7);
        public DateTime? EndDate { get; set; } = DateTime.Now;
    }
}
