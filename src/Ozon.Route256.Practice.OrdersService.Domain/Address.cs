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

    public static Address CreateInstance(string json)
    {
        try
        {
            var address = JsonSerializer.Deserialize<Address>(json);
            if (address is not null)
                return address;
            throw new Exception();
        }
        catch
        {
            throw new DomainException($"Ошибка при десериализации сущности Address");
        }
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public string Region { get; init; } = default!;
    public string City { get; init; } = default!;
    public string Street { get; init; } = default!;
    public string Building { get; init; } = default!;
    public string Apartment { get; init; } = default!;
    public Coordinates Coordinates { get; init; } = default!;

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
