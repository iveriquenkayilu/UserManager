namespace UserManagerService.Entities.Interfaces
{
    public interface IAddress : IBaseEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        string Country { get; set; }
        string State { get; set; }
        string City { get; set; }
        string Street { get; set; }
        string Building { get; set; }
        string Number { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
    }
}
