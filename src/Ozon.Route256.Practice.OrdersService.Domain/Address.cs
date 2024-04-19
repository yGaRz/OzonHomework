using Ozon.Route256.Practice.OrdersService.Domain.Core;
using System.Text.Json;

namespace Ozon.Route256.Practice.OrdersService.Domain;

public sealed class Address : ValueObject
{
    public Address() { }
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

    public static Address? CreateInstance(string json)
    {
        return JsonSerializer.Deserialize<Address>(json);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public string Region { get; init; }
    public string City { get; init; }
    public string Street { get; init; }
    public string Building { get; init; }
    public string Apartment { get; init; }
    public Coordinates Coordinates { get; init; }

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
