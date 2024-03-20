namespace Ozon.Route256.Practice.GatewayService.Etities;

    public record CustomerEntity(
        int Id,
        string FirstName,
        string LastName,
        string MobileNumber,
        string Email,
        AddressEntity DefaultAddress,
        AddressEntity[] Addressed);

