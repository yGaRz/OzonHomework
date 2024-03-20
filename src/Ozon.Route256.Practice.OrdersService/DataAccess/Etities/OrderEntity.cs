using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.OrdersService.Models;
namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

public record OrderEntity
{
    public long Id { get; init; }
    public OrderSource Source { get;init; }
    public CustomerEntity Customer { get; init; }
    public IEnumerable<GoodEntity> Goods { get; init; }
    public OrderEntity(int  id, OrderSource source, OrderStateEnum state, CustomerEntity customer, IEnumerable<GoodEntity> goods)
    {
        Id = id; 
        Source = source; 
        State = state; 
        Customer = customer;
        Goods = goods;
        TimeCreate = Timestamp.FromDateTime(DateTime.UtcNow);
        TimeUpdate = Timestamp.FromDateTime(DateTime.UtcNow);
    }
    //Изменяемые поля
    public OrderStateEnum State { get; set; }
    /// <summary>
    /// Время создания заказа
    /// </summary>
    public Timestamp TimeCreate { get; set; }

    /// <summary>
    /// Время изменения последнего стауса
    /// </summary>
    public Timestamp TimeUpdate { get; set; }
}

