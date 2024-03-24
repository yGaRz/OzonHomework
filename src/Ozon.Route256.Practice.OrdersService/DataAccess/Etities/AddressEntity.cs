namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

public record AddressEntity
{
    public string Region { get; init; }
    public string City { get; init; }
    public string Street { get; init; }
    public string Building { get; init; }
    public string Apartment { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public AddressEntity(string region, string city, string street, string building, string apartment, double lat, double lon)
    {
        Region = region;
        City = city;
        Street = street;
        Building = building;
        Apartment = apartment;
        Latitude = lat;
        Longitude = lon;
    }
}


