namespace Ozon.Route256.Practice.GatewayService
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddControllers();
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddSwaggerGen();

        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRouting();
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI();
        }

    }
}
