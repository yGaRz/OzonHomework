using Ozon.Route256.Practice.CustomerGprcFile;
namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
//Чтобы можно было сгенерировать пользователей, по факту костыль для тестов.
public record AddressDal
{
    public string Region { get; set; }
    public string City { get; init; }
    public string Street { get; init; }
    public string Building { get; init; }
    public string Apartment { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public AddressDal(string region, string city, string street, string building, string apartment, double latitude, double longitude)
    {
        Region = region;
        City = city;
        Street = street;
        Building = building;
        Apartment = apartment;
        Latitude = latitude;
        Longitude = longitude;
    }

    public static AddressDal ConvertFromAddressGrpc(Address address)
    {
        return new AddressDal(address.Region, address.City, address.Street, address.Building, address.Apartment, address.Latitude, address.Longitude);
    }
    public static Address ConvertToAddressGrpc(AddressDal address)
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


