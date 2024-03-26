using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.OrdersService.Models;
namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

public record OrderEntity
{
    public long Id { get; init; }
    public OrderSourceEnum Source { get; init; }
    public CustomerEntity Customer { get; init; }
    public IEnumerable<ProductEntity> Goods { get; init; }
    public int CountGoods => Goods.Count();
    public double TotalSum => Goods.Sum(x => x.Price * x.Quantity);
    public double TotalWeigth => Goods.Sum(x => x.Weight);
    public OrderStateEnum State { get; set; }
    /// <summary>
    /// Время создания заказа
    /// </summary>
    public DateTime TimeCreate { get; set; }

    /// <summary>
    /// Время изменения последнего стауса
    /// </summary>
    public DateTime TimeUpdate { get; set; }

    public OrderEntity(int id, OrderSourceEnum source, OrderStateEnum state, CustomerEntity customer, IEnumerable<ProductEntity> goods)
    {
        Id = id;
        Source = source;
        State = state;
        Customer = customer;
        Goods = goods;
        TimeCreate = DateTime.UtcNow;
        TimeUpdate = DateTime.UtcNow;
    }

    public static Order ConvertOrder(OrderEntity order)
    {
        var orderEntity = new Order()
        {
            CountGoods = order.CountGoods,
            DateCreate = order.TimeCreate.ToTimestamp(),
            Id = order.Id,
            TotalWeight = order.TotalWeigth,
            OrderSource = (OrderSource)order.Source,
            OrderState = (OrderState)order.State,
            TotalSum = order.TotalSum
        };
        foreach (var g in order.Goods)
        {
            orderEntity.ProductList.Add(new Product()
            {
                Id = g.Id,
                Name = g.Name,
                Quantity = g.Quantity,
                Price = g.Price,
                Wight = g.Weight
            });
        }
        return orderEntity;
    }
}


