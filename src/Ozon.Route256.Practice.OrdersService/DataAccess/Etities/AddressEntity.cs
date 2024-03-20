namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
public record AddressEntity(
        string Region,
        string City,
        string Street,        
        string Building,
        string Apartment,
        double Latitude,
        double Longitude);

