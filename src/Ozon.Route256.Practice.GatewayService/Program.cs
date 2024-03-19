using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace Ozon.Route256.Practice.GatewayService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
                .Build()
                .RunAsync();
        }
    }
}