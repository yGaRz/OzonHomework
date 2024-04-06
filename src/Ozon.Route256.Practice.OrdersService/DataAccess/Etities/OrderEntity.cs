using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.CustomerGprcFile;
using Ozon.Route256.Practice.OrdersGrpcFile;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

public record OrderEntity
{
    public long Id { get; init; }
    public int CustomerId { get; set; }
    public OrderSourceEnum Source { get; init; }
    public AddressEntity Address { get; init; }
    public List<ProductEntity> Goods { get; init; }=new List<ProductEntity>();
    public OrderStateEnum State { get; set; }
    public DateTime TimeCreate { get; set; }
    public DateTime TimeUpdate { get; set; }
    public string Region { get; set; }
    public int CountGoods { get; init; }
    public double TotalPrice { get; init; }
    public double TotalWeigth { get; init; }
    public OrderEntity(long id, OrderSourceEnum source, OrderStateEnum state, int customerId, Address address, IEnumerable<ProductEntity> goods)
    {
        Id = id;
        Source = source;
        State = state;
        CustomerId = customerId;
        Address = AddressEntity.ConvertFromAddressGrpc(address);
        Region = Address.Region;
        Goods = new List<ProductEntity>();
        Goods.AddRange(goods);  
        CountGoods = Goods.Count();
        TotalPrice = Goods.Sum(x => x.Price * x.Quantity);
        TotalWeigth = Goods.Sum(x => x.Weight);
        TimeCreate = DateTime.UtcNow;
        TimeUpdate = DateTime.UtcNow;
    }
    public OrderEntity(long id, OrderSourceEnum source, OrderStateEnum state, int customerId, AddressEntity address, IEnumerable<ProductEntity> goods)
    {
        Id = id;
        Source = source;
        State = state;
        CustomerId = customerId;
        Address = address;
        Region = Address.Region;
        Goods = new List<ProductEntity>();
        Goods.AddRange(goods);
        CountGoods = Goods.Count();
        TotalPrice = Goods.Sum(x => x.Price * x.Quantity);
        TotalWeigth= Goods.Sum(x => x.Weight);
        TimeCreate = DateTime.UtcNow;
        TimeUpdate = DateTime.UtcNow;
    }


#pragma warning disable CS8618,CS8602,CS8604,CS8601 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    public OrderEntity(long id, string message,DateTime timeCreate)    
    {
        try
        {
            var doc = JsonNode.Parse(message);
            Id = id;
            TimeCreate = timeCreate;
            TimeUpdate = timeCreate;
            Source = (OrderSourceEnum)((int)doc["Source"] - 1);
            var customer = doc["Customer"];
            var goods = doc["Goods"].AsArray();
            CustomerId = (int)customer["Id"];
            var address = customer["Address"];
            Address = JsonSerializer.Deserialize<AddressEntity>(address);
            Region = Address.Region;
            foreach (var good in goods)
            {
                var d = JsonSerializer.Deserialize<ProductEntity>(good.ToJsonString());
                Goods.Add(d);
            }
            CountGoods = Goods.Count();
            TotalPrice = Goods.Sum(x => x.Price * x.Quantity);
            TotalWeigth = Goods.Sum(x => x.Weight);
        }
        catch
        {
            throw new ArgumentException($"Ошибка при десериализации данных ID={id} message = {message}");
        }
    }

    public OrderEntity()
    {
    }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

    public static Order ConvertToOrderGrpc(OrderEntity order)
    {
        var orderEntity = new Order()
        {
            CountGoods = order.CountGoods,
            DateCreate = order.TimeCreate.ToTimestamp(),
            Id = order.Id,
            TotalWeight = order.TotalWeigth,
            OrderSource = (OrderSource)order.Source,
            OrderState = (OrderState)order.State,
            TotalSum = order.TotalPrice
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


