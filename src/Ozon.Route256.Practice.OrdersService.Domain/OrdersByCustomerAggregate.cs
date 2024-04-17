using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Route256.Practice.OrdersService.Domain;
//TODO: Агрегат для получения списков заказов по клиенту
public sealed class OrdersByCustomerAggregate
{
    private OrdersByCustomerAggregate(Customer customer, IEnumerable<Order> orders)
    {
        _customer = customer;
        _orders = orders;
    }
    private Customer _customer;
    private IEnumerable<Order> _orders;  
}
