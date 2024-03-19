using Grpc.Core;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices
{
    public class LogisticsService: LogisticsSimulatorService.LogisticsSimulatorServiceBase
    {
        public override Task<CancelResult> OrderCancel(LogisticsSimulator.Grpc.Order request, ServerCallContext context)
        {
            return base.OrderCancel(request, context);
        }

    }
}
