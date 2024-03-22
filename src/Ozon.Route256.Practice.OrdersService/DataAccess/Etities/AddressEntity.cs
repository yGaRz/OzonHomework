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
}


