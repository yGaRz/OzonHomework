namespace Ozon.Route256.Practice.OrdersService.GrpcServices;
public static partial class ResponseTimeLoggerExtension
{
    [LoggerMessage(LogLevel.Debug, Message = "Response received in {milliseconds}ms")]
    public static partial void LogResponseTime(this ILogger logger, long milliseconds);
}
