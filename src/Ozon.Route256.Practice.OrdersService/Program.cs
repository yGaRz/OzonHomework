using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common;
using System.Net;
using Serilog;
using static System.Int32;

namespace Ozon.Route256.Practice.OrdersService;
public class Program
{       
    public static async Task Main(string[] args)
    {
        await Host
           .CreateDefaultBuilder(args)
           .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>()
               .ConfigureKestrel(option =>
               {
                   option.ListenPortByOptions(ProgramExtension.ROUTE256_GRPC_PORT, HttpProtocols.Http2);
               }))
           .UseSerilog()
           .Build()
           .RunOrMigrateAsync(args);
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
    public static async Task RunOrMigrateAsync(
                        this IHost host,
                        string[] args)
    {
        if (args.Length > 0 && args[0].Equals("migrateUp", StringComparison.InvariantCultureIgnoreCase))
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IShardMigrator>();
            await runner.MigrateUp(cts.Token);
        }
        else if (args.Length > 0 && args[0].Equals("migrateDown", StringComparison.InvariantCultureIgnoreCase))
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IShardMigrator>();
            await runner.MigrateDown(0,cts.Token);
        }
        else
            host.Run();
    }
}