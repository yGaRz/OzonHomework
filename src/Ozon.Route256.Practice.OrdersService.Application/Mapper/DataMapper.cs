using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Domain;

namespace Ozon.Route256.Practice.OrdersService.Application.Mapper;
internal class DataMapper : ICommandMapper
{
    public PreOrder PreOrderToDomain(PreOrderDto preOrderDto)
    {
        return PreOrder.CreateInstance(id: preOrderDto.Id,
            source: preOrderDto.Source,
            customerId: preOrderDto.CustomerId,
            address: AddressToDomain(preOrderDto.Address),
            products: ProductsToDomain(preOrderDto.Goods),
            created: preOrderDto.TimeCreate);
    }

    private static Address AddressToDomain(AddressDto address)
    => Address.CreateInstance(
        region: address.Region,
        city: address.City,
        street: address.Street,
        building: address.Building,
        apartment: address.Apartment,
        coordinates: Coordinates.CreateInstance(
            latitude: address.Latitude,
            longitude: address.Longitude));

    private static IEnumerable<Product> ProductsToDomain(IEnumerable<ProductDto> products)
    {
        List<Product> result = products.Select(x=>Product.CreateInstance(x.Id,x.Name,x.Quantity,x.Price,x.Weight)).ToList();
        return result;
    }

    public Order OrderToDomain(OrderDto orderDto)
    {
        try
        {
            var address = Address.CreateInstance(orderDto.address.Region,
                                                    orderDto.address.City,
                                                    orderDto.address.Street,
                                                    orderDto.address.Building,
                                                    orderDto.address.Apartment,
                                                    Coordinates.CreateInstance(orderDto.address.Latitude,orderDto.address.Longitude));
            return Order.CreateInstace(orderDto.id,
                orderDto.customer_id,
                orderDto.source,
                address,
                orderDto.state,
                orderDto.timeCreate,
                orderDto.timeUpdate,
                address.Region,
                orderDto.countGoods,
                orderDto.totalPrice,
                orderDto.totalWeigth);
        }
        catch
        {
            throw new DomainException($"Ошибка при десериализации заказа {orderDto.id}");
        }
    }
}
