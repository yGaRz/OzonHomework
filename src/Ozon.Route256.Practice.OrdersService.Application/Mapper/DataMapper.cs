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
}
