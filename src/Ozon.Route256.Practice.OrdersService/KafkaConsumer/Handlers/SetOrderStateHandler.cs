using Ozon.Route256.Practice.OrdersService.Application;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using Ozon.Route256.Practice.OrdersService.Kafka.Consumer;

namespace Ozon.Route256.Practice.OrdersService.Kafka.ProducerNewOrder.Handlers;

internal class SetOrderStateHandler : ISetOrderStateHandler
{
    private readonly IOrderServiceAdapter _orderServiceAdapter;
    private readonly ILogger<AddOrderHandler> _logger;

    public SetOrderStateHandler(  ILogger<AddOrderHandler> logger, IOrderServiceAdapter orderServiceAdapter)
    {
        _logger = logger;
        _orderServiceAdapter = orderServiceAdapter;
    }
    public async Task Handle(long id, OrderStateEnumDomain state, DateTime timeUpdate, CancellationToken token)
    {
        try
        {
            await _orderServiceAdapter.SetOrderStateAsync(id, state, timeUpdate, token);
            //_logger.LogInformation($"{id}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex.Message);
        }
    }
}
