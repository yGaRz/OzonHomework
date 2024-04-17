using Ozon.Route256.Practice.OrdersService.Domain.Core;

namespace Ozon.Route256.Practice.OrdersService.Domain;

public sealed class Address : ValueObject
{
    private Address(string region, string city, string street, string building, string apartment, Coordinates coordinates)
    {
        Region = region;
        City = city;
        Street = street;
        Building = building;
        Apartment = apartment;
        Coordinates = coordinates;
    }

    public static Address CreateInstance(string region, string city, string street, string building, string apartment, Coordinates coordinates)
    {
        return new Address(region, city, street, building, apartment, coordinates);
    }

    public string Region { get; }

    public string City { get; }

    public string Street { get; }

    public string Building { get; }

    public string Apartment { get; }

    public Coordinates Coordinates { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Region;
        yield return City;
        yield return Street;
        yield return Building;
        yield return Apartment;
        yield return Coordinates;
    }
}
