using Bogus;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

namespace TestServices.Helpers
{
    public class OrderGenerator
    {
        public static OrderEntity GenerateOrder(int idCustomer, int idOrder)
        {
            CustomerEntity customer = new CustomerEntity();
            Faker faker = new Faker();
            AddressEntity address = new AddressEntity(faker.Address.Country(),
                                                faker.Address.City(),
                                                faker.Address.StreetName(),
                                                faker.Address.BuildingNumber(),
                                                faker.Address.StreetSuffix(),
                                                faker.Address.Latitude(),
                                                faker.Address.Longitude());
            CustomerEntity cusromer = new CustomerEntity()
            {
                Id = idCustomer,
                DefaultAddress = address
            };
            List<ProductEntity> goods = new List<ProductEntity>();
            int cnt = faker.Random.Int(1,4);
            for (int j = 0; j < cnt; j++)
            {
                ProductEntity good = new ProductEntity(faker.Random.Int(0, 555555),
                    faker.Commerce.Product(),
                    faker.Random.Int(1, 5),
                    faker.Random.Double(1, 500),
                    faker.Random.UInt(1, 20)
                    );
                goods.Add(good);
            }

            return new OrderEntity(idOrder, OrderSourceEnum.WebSite, OrderStateEnum.Created, customer.Id,customer.DefaultAddress, goods);
        }
    }
}
