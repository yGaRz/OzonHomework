﻿namespace Ozon.Route256.Practice.GatewayService.Models
{
    public record OrdersListModel
    {        
        public uint PageIndex { get; set; }
        public List<Order> ListOrder { get; set; } = new List<Order>();
    }
}