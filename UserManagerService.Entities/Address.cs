namespace UserManagerService.Entities
{
    public class Address : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
