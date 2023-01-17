namespace UserManagerService.Entities
{
	public class LoginSession : BaseCompanyEntity
	{
		public string Location { get; set; } // Lat Lng
		public string Status { get; set; }
		public string Device { get; set; }
		public string IpAdress { get; set; }
	}
}