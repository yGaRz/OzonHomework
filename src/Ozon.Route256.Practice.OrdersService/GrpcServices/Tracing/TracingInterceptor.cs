using Grpc.Core.Interceptors;
using Grpc.Core;
using System.Diagnostics;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices;
public class TracingInterceptor : Interceptor
{
    internal static readonly string grpc_interceptor_activity = "Grpc Interceptor";

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        using var grpcActivity = new ActivitySource(grpc_interceptor_activity).StartActivity(
            name: context.Method,
            kind: ActivityKind.Internal,
            tags: new List<KeyValuePair<string, object?>>
            {
                new ("grpc_request", request),
                new ("grpc_headers", context.RequestHeaders)
            });

        return base.UnaryServerHandler(request, context, continuation);
    }
}
