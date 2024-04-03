using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using FluentMigrator.Runner;
using static System.Int32;
using System.Net;

namespace Ozon.Route256.Practice.OrdersService;
public class Program
{       
    public static void Main(string[] args)
    {
        Host
           .CreateDefaultBuilder(args)
           .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>()
               .ConfigureKestrel(option =>
               {
                   option.ListenPortByOptions(ProgramExtension.ROUTE256_GRPC_PORT, HttpProtocols.Http2);
               }))
           .Build()
           .RunOrMigrate(args);
    }
}
public static class ProgramExtension
{
    public const string ROUTE256_GRPC_PORT = "ROUTE256_GRPC_PORT";

    public static void ListenPortByOptions(
        this KestrelServerOptions option,
        string envOption,
        HttpProtocols httpProtocol)
    {
        var isHttpPortParsed = TryParse(Environment.GetEnvironmentVariable(envOption), out var httpPort);
        if (isHttpPortParsed)
            option.Listen(IPAddress.Any, httpPort, options => options.Protocols = httpProtocol);
    }
    public static void RunOrMigrate(
                        this IHost host,
                        string[] args)
    {
        if (args.Length > 0 && args[0].Equals("migrateUp", StringComparison.InvariantCultureIgnoreCase))
        {
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
        else if (args.Length > 0 && args[0].Equals("migrateDown", StringComparison.InvariantCultureIgnoreCase))
        {
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateDown(0);
        }
        else
            host.Run();
    }
}