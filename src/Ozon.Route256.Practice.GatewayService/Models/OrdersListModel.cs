using Ozon.Route256.Practice.OrdersGrpcFile;

namespace Ozon.Route256.Practice.GatewayService.Models
{
    public record OrdersListModel
    {        
        public List<Order> ListOrder { get; set; } = new List<Order>();
    }
}
