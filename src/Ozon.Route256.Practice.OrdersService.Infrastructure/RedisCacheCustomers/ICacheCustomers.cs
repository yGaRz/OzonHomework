﻿using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
internal interface ICacheCustomers
{
    Task<CustomerDal?> Find(int id, CancellationToken cancellationToken);
    Task Insert(CustomerDal customer, CancellationToken cancellationToken);
    Task<bool> IsExists(int id, CancellationToken cancellationToken);
}
