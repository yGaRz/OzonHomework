using Ozon.Route256.Practice.OrdersService.Domain.Core;
using System.Text.Json;

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
        if (region is null || region.Length == 0)
            throw new DomainException("Ошибка в названии региона");
        if (city is null || city.Length == 0)
            throw new DomainException("Ошибка в названии города");
        if (street is null || street.Length == 0)
            throw new DomainException("Ошибка в названии улицы");
        if (building is null || building.Length == 0)
            throw new DomainException("Ошибка в названии номера дома");
        if (apartment is null)
            apartment = "";
        return new Address(region, city, street, building, apartment, coordinates);
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
