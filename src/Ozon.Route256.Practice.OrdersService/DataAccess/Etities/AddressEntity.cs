namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

public record AddressEntity
{
    public string Region { get; set; }
    public string City { get; init; }
    public string Street { get; init; }
    public string Building { get; init; }
    public string Apartment { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public AddressEntity(string region, string city, string street, string building, string apartment, double latitude, double longitude)
    {
        Region = region;
        City = city;
        Street = street;
        Building = building;
        Apartment = apartment;
        Latitude = latitude;
        Longitude = longitude;
    }

    public static AddressEntity ConvertFromAddressGrpc(Address address)
    {
        return new AddressEntity(address.Region,address.City,address.Street,address.Building,address.Apartment,address.Latitude,address.Longitude);
    }
    public static Address ConvertToAddressGrpc(AddressEntity address)
    {
        return new Address()
        {
            Region = address.Region,
            City = address.City,
            Street = address.Street,
            Building = address.Building,
            Apartment = address.Apartment,
            Latitude = address.Latitude,
            Longitude = address.Longitude
        };
    }

}


