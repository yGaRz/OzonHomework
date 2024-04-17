using Ozon.Route256.Practice.OrdersService.Domain;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using System.Text.Json.Nodes;
using System.Text.Json;

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
    public PreOrderDto(long id, string message, DateTime timeCreate)
    {
        try
        {
            Id = id;
            TimeCreate = timeCreate;
            var doc = JsonNode.Parse(message);
            Source = (OrderSourceEnumDomain)((int)doc["Source"] - 1);
            var customer = doc["Customer"];
            var goods = doc["Goods"].AsArray();
            CustomerId = (int)customer["Id"];
            var address = customer["Address"];
            Address = JsonSerializer.Deserialize<AddressDto>(address);
            foreach (var good in goods)
            {
                var d = JsonSerializer.Deserialize<ProductDto>(good.ToJsonString());
                Goods.Add(d);
            }
        }
        catch
        {
            throw new ArgumentException($"Ошибка при десериализации данных ID={id} message = {message}");
        }
    }
    public long Id { get; init; }
    public int CustomerId { get; set; }
    public OrderSourceEnumDomain Source { get; init; }
    public AddressDto Address { get; init; }
    public List<ProductDto> Goods { get; init; } = new List<ProductDto>();
    public OrderStateEnumDomain State { get; init; }
    public DateTime TimeCreate { get; init; }

}
