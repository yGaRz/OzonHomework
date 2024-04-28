using Grpc.Core.Interceptors;
using Grpc.Core;
using System.Diagnostics;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices.Metrics;

internal class MetricsInterceptor : Interceptor
{
    private readonly IGrpcMetrics _grpcMetrics;
    private readonly GrpcCounterMetrics _grpcCounterMetrics;
    public MetricsInterceptor(IGrpcMetrics grpcMetrics, GrpcCounterMetrics grpcCounterMetrics)
    {
        _grpcMetrics = grpcMetrics;
        _grpcCounterMetrics = grpcCounterMetrics;
    }
    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = base.UnaryServerHandler(request, context, continuation);
            stopwatch.Stop();

            _grpcMetrics.WriteResponseTime(context.Method, stopwatch.ElapsedMilliseconds, isSuccess: true);
            _grpcCounterMetrics.Request(context.Method);
            return result;
        }
        catch
        {
            stopwatch.Stop();
            _grpcMetrics.WriteResponseTime(context.Method, stopwatch.ElapsedMilliseconds, isSuccess: false);
            throw;
        }
    }
}
