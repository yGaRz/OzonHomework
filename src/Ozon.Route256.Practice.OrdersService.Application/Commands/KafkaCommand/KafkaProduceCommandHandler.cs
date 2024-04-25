using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Commands.KafkaQuery;

namespace Ozon.Route256.Practice.OrdersService.Application.Commands.KafkaCommand;

internal class KafkaProduceCommandHandler : IRequestHandler<KafkaProduceCommand, Unit>
{
    IKafkaProduceAdapter _produceAdapter;
    public KafkaProduceCommandHandler(IKafkaProduceAdapter produceAdapter)
    {
        _produceAdapter = produceAdapter;
    }
    public async Task<Unit> Handle(KafkaProduceCommand request, CancellationToken cancellationToken)
    {
        await _produceAdapter.ProduceAsync(request, cancellationToken);
        return Unit.Value;
    }
}
