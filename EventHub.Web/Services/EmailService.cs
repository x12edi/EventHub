using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace EventHub.Web.Services
{
    public class EmailService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Checking for upcoming events...");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}