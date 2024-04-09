using System.Data.Common;
using Npgsql;

namespace Ozon.Route256.Practice.OrdersService.DAL.Common;

public static class NpgsqlExtensions
{
    public static void Add<T>(this DbParameterCollection parameters, string name, T? value) =>
        parameters.Add(
            new NpgsqlParameter<T>
            {
                ParameterName = name,
                TypedValue    = value
            });
}