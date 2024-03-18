namespace Ozon.Route256.Practice.GatewayService.Models
{
    public class OrdersListModel
    {
        public uint PageIndex {  get; set; }
        public List<Order> ListOrder { get; set; }
        public OrdersListModel() { 
            ListOrder = new List<Order>();
        }
    }
}
