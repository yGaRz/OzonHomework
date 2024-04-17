using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Route256.Practice.OrdersService.Domain.Core;
public abstract class EntityInt : Entity<int>
{
    protected EntityInt()
    {
    }

    protected EntityInt(int id)
        : base(id)
    {
    }
}
