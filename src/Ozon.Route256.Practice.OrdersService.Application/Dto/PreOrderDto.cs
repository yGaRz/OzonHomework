using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Ozon.Route256.Practice.OrdersService.Application.Dto;
public class PreOrderDto
{
    private PreOrderDto(long id, int customerId, OrderSourceEnumDomain source, AddressDto address, List<ProductDto> goods, OrderStateEnumDomain state, DateTime timeCreate)
    {
        Id = id;
        CustomerId = customerId;
        Source = source;
        Address = address;
        Goods.AddRange(goods);
        State = state;
        TimeCreate = timeCreate;
    }
#pragma warning disable CS8618, CS8602, CS8604 , CS8601 
    public PreOrderDto(long id, string message, DateTime timeCreate)
    {
        try
        {
            Id = id;
            TimeCreate = timeCreate;
            var doc = JsonNode.Parse(message);
            Source = (OrderSourceEnumDomain)((int)doc["Source"] - 1);

            var customer = doc["Customer"];
            if(customer == null)
                throw new ArgumentException($"В заказе ID={id} не указан клиент");
            if(customer["Id"]!=null && int.TryParse(customer["Id"].ToString(), out int customerId))
                CustomerId = customerId;
            else
                throw new ArgumentException($"В заказе ID={id} не указан идентификатор клиента");

            if (doc["Goods"] != null)
            {
                var goods = doc["Goods"].AsArray();
                foreach (var good in goods)
                {
                    var d = JsonSerializer.Deserialize<ProductDto>(good.ToJsonString());
                    Goods.Add(d);
                }
            }
            else
                throw new ArgumentException($"В заказе ID={id} нет товаров");
            var address = customer["Address"];
            Address = JsonSerializer.Deserialize<AddressDto>(address);

        }
        catch
        {
            throw new ArgumentException($"Ошибка при десериализации данных ID={id} message = {message}");
        }
    }
#pragma warning restore CS8618, CS8602, CS8604, CS8601 
    public long Id { get; init; }
    public int CustomerId { get; set; }
    public OrderSourceEnumDomain Source { get; init; }
    public AddressDto Address { get; init; }
    public List<ProductDto> Goods { get; init; } = new List<ProductDto>();
    public OrderStateEnumDomain State { get; init; }
    public DateTime TimeCreate { get; init; }

}
