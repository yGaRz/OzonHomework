using Grpc.Core;
namespace Ozon.Route256.Practice.GatewayService.Model;

public enum OrderState
{
    Created=0,
    SentToCustomer=1,
    Delivered=2,
    Lost = 3,
    Cancelled=4
}