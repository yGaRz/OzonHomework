
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using static System.Int32;
using System.Net;

namespace Ozon.Route256.Practice.OrdersService
{
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
                .Build()
                .RunAsync();
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
    }
}