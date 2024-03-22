namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities
{
    public record CustomerEntity
    {
        public int Id { get; init; }
        public AddressEntity Address { get; init; }
    }
}
