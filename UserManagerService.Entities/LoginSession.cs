using System;

namespace UserManagerService.Entities
{
	public class LoginSession : BaseCompanyEntity
	{
		public Guid AddressId { get; set; }
		public virtual Address Address { get; set; }
		public string Status { get; set; }
		public string Device { get; set; }
		public string IpAddress { get; set; }
	}
}