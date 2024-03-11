using Grpc.Core;

namespace Ozon.Route256.Practice.OrdersService.Exceptions
{
    public class NotFoundException : Exception    
    {
        public override string Message { get; }
        public NotFoundException(string exceptionMessage)
        {            
            Message = exceptionMessage;
        }
    }
}
