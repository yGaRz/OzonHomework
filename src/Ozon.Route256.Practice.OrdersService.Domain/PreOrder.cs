using Ozon.Route256.Practice.OrdersService.Domain.Core;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Route256.Practice.OrdersService.Domain;
public sealed class PreOrder:Entity<long>
{
    private PreOrder(long  id, OrderSourceEnumDomain source, int customerId, Address address, IEnumerable<Product> products, DateTime created):base(id)
    {
        _source = source;
        _customerId = customerId;
        _address = address;
        _products = products.ToList();
        _created = created;
    }

    public static PreOrder CreateInstance(long id, OrderSourceEnumDomain source, int customerId, Address address, IEnumerable<Product> products, DateTime created)
    {
         return new PreOrder(id, source, customerId, address, products, created);
    }

    public Order CreateOrder()
    {
        return Order.CreateInstace(
            id: Id,
            customerId: _customerId,
            source: _source,
            addressOrder: _address,
            state: OrderStateEnumDomain.Created,
            timeCreate: _created,
            timeUpdate: _created,
            region: _address.Region,
            countGoods: _products.Count(),
            totalPrice: _products.Sum(x => x.Price * x.Quantity),
            totalWeigth: _products.Sum(x => x.Weight * x.Quantity));
    }
    private DateTime _created;
    private OrderSourceEnumDomain _source { get;}
    private  int _customerId {  get;}
    private Address _address { get;}
    private List<Product> _products { get;}

}
