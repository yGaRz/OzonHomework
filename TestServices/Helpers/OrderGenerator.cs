namespace TestServices.Helpers;

//public class OrderGenerator
//{
//    public static OrderDao GenerateOrder(int idCustomer, int idOrder)
//    {
//        CustomerDto customer = new CustomerDto();
//        Faker faker = new Faker();
//        Ozon.Route256.Practice.OrdersService.Application.Dto.AddressDto address = new Ozon.Route256.Practice.OrdersService.Application.Dto.AddressDto(faker.Address.Country(),
//                                            faker.Address.City(),
//                                            faker.Address.StreetName(),
//                                            faker.Address.BuildingNumber(),
//                                            faker.Address.StreetSuffix(),
//                                            faker.Address.Latitude(),
//                                            faker.Address.Longitude());
//        CustomerDto cusromer = new CustomerDto()
//        {
//            Id = idCustomer,
//            DefaultAddress = address
//        };
//        List<ProductDto> goods = new List<ProductDto>();
//        int cnt = faker.Random.Int(1,4);
//        for (int j = 0; j < cnt; j++)
//        {
//            ProductDto good = new ProductDto(faker.Random.Int(0, 555555),
//                faker.Commerce.Product(),
//                faker.Random.Int(1, 5),
//                faker.Random.Double(1, 500),
//                faker.Random.UInt(1, 20)
//                );
//            goods.Add(good);
//        }

//        return new OrderDao(idOrder, OrderSourceEnum.WebSite, OrderStateEnum.Created, customer.Id,customer.DefaultAddress, goods);
//    }
//}
