namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities
{
    public record CustomerEntity(
        int Id,
        string FirstName,
        string LastName,
        string MobileNumber,
        string Email,
        Address DefaultAddress,
        Address[] Addressed);
}
