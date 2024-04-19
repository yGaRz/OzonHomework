using Ozon.Route256.Practice.OrdersService.Domain.Core;

namespace Ozon.Route256.Practice.OrdersService.Domain;

public sealed class Coordinates : ValueObject
{
    public Coordinates() { }
    private Coordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Coordinates CreateInstance(double latitude, double longitude)
    {
        if (latitude > 90 || latitude < -90)
            throw new DomainException("Incorrect latitude");
        if (longitude > 180 || longitude < -180)
            throw new DomainException("Incorrect longitude");
        return new Coordinates(latitude, longitude);
    }

    public double Latitude { get; init; }

    public double Longitude { get; init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}
