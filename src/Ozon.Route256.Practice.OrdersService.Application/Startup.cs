using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Practice.OrdersService.Application.Mapper;
using System.Reflection;
namespace Ozon.Route256.Practice.OrdersService.Application;
public static class Startup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        _ = services.AddSingleton<ICommandMapper, DataMapper>();
        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}
