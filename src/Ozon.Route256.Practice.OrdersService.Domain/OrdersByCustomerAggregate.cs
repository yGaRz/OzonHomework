using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Route256.Practice.OrdersService.Domain;
public sealed class OrdersByCustomerAggregate
{
    private OrdersByCustomerAggregate(Customer customer, IEnumerable<Order> orders, Address address)
    {
        _customer = customer;
        _orders = orders;
        _address = address;
    }

    private Customer _customer;
    private IEnumerable<Order> _orders;     
    private Address _address;
    public static OrdersByCustomerAggregate CreateInstance(Customer customer, IEnumerable<Order> orders, Address address)
    {
        return new OrdersByCustomerAggregate(customer, orders, address);
    }
    public string CustomerFullName { get => $"{_customer.FirstName} {_customer.LastName}"; }
    public string CustomerPhone { get => $"{_customer.MobileNumber}"; }
    public string Region { get => $"{_address.Region}"; }    
    public Address Address { get => _address; }

    public IReadOnlyCollection<Order> Orders { get => (IReadOnlyCollection<Order>)_orders; }
}
