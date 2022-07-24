using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagerService.Entities
{
    public class LoginSession: BaseEntity
    {
        public string Location { get; set; } // Lat Lng
        public string Status { get; set; }
        public string Device { get; set; }
        public string IpAdress { get; set; }
    }
}
