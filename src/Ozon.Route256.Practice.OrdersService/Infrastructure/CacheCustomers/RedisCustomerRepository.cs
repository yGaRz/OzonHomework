using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using System.Text.Json.Serialization;
using System.Text.Json;
using StackExchange.Redis;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.Exceptions;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers
{
    public class RedisCustomerRepository : ICacheCustomers
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };
        private readonly IDatabase _database;
        private readonly IServer _server;

        public RedisCustomerRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase(0);
            _server = connectionMultiplexer.GetServers()[0];
        }
        public async Task<CustomerEntity?> Find(int id, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            var key = BuildCustomerKey(id);
            var resultRedis = await _database.StringGetAsync(key);

            var result = resultRedis.HasValue ? JsonSerializer.Deserialize<CustomerEntity>(resultRedis.ToString(), _jsonSerializerOptions) : null;
            return result;
        }

        public async Task Insert(CustomerEntity customer, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var key = BuildCustomerKey(customer.Id);

            if (_database.KeyExists(key))
                throw new RepositoryException($"Customer with id = {customer.Id} already exists");

            var resultRedis = JsonSerializer.Serialize(customer, _jsonSerializerOptions);
            await _database.StringSetAsync(key, resultRedis);
        }

        public async Task<bool> IsExists(int id, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return false;

            var key = BuildCustomerKey(id);
            var contains = await _database.KeyExistsAsync(key);
            return contains;
        }

        private static RedisKey BuildCustomerKey(int customerId)
        {
            return new RedisKey($"customer:{customerId}");
        }
    }
}
