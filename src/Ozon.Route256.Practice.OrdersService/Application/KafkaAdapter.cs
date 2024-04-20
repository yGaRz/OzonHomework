using MediatR;
using Ozon.Route256.Practice.OrdersService.Application;
using Ozon.Route256.Practice.OrdersService.Application.Commands.KafkaQuery;

namespace Ozon.Route256.Practice.OrdersService.Kafka.KafkaProducerNewOrder;

public class KafkaAdapter : IKafkaAdapter
{
    private readonly IMediator _mediator;
    public KafkaAdapter(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ProduceAsync(IReadOnlyCollection<long> updatedOrders, CancellationToken token)
    {
        KafkaProduceCommand command = new KafkaProduceCommand()
        {
            updatedOrders = updatedOrders,
        };
        await _mediator.Send(command, token);
    }
}
