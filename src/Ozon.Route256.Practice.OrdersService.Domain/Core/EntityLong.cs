﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Route256.Practice.OrdersService.Domain.Core;

public abstract class EntityLong : Entity<long>
{

    protected EntityLong(long id)
        : base(id)
    {
    }
}
