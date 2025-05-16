using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderEventProcessor.Persistence;
using OrderEventProcessor.Services;

namespace OrderEventProcessor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Start RabbitMqListener
            var listener = host.Services.GetRequiredService<RabbitMqListener>();
            await listener.StartListeningAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>();
                    services.AddSingleton<RabbitMqListener>();
                });
    }
}