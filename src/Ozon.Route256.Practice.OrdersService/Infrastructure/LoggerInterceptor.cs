using Grpc.Core;
using Grpc.Core.Interceptors;
using Ozon.Route256.Practice.OrdersService.Exceptions;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure
{
    public sealed class LoggerInterceptor:Interceptor
    {
        private readonly ILogger<LoggerInterceptor> _logger;
        public LoggerInterceptor(ILogger<LoggerInterceptor> logger)
        {
            _logger = logger;
        }
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
                            TRequest request,
                            ServerCallContext context,
                            UnaryServerMethod<TRequest,
                            TResponse> continuation)
        {
            _logger.LogInformation("Request {request}", request);
            try
            {
                var response = await base.UnaryServerHandler(request, context, continuation);
                _logger.LogInformation("Response {response}", response);
                return response;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Rpc exception happened");
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Not Found  exception happened");
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some exception happened");
                throw new RpcException(new Status(StatusCode.Unknown, ex.Message));
            }
        }
    }
}
