using Grpc.Core;
using Swashbuckle.AspNetCore.Annotations;
namespace Ozon.Route256.Practice.GatewayService.Models
{
    public enum OrderStateEnum
    {
        Created = 0,
        SentToCustomer = 1,
        Delivered = 2,
        Lost = 3,
        Cancelled = 4
    }
}