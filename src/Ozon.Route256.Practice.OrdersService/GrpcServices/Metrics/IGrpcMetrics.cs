namespace Ozon.Route256.Practice.OrdersService.GrpcServices.Metrics;

internal interface IGrpcMetrics
{    
    void WriteResponseTime(string method, long elapsedMs, bool isSuccess);
}
