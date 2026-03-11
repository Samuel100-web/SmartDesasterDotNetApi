using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartResponse.Core.Interfaces;

namespace SmartResponse.Infrastructure.Services
{
    public class DisasterSyncWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DisasterSyncWorker> _logger;

        public DisasterSyncWorker(IServiceProvider serviceProvider, ILogger<DisasterSyncWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Disaster Sync Worker running at: {time}", DateTimeOffset.Now);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var syncService = scope.ServiceProvider.GetRequiredService<IExternalSyncService>();
                    await syncService.SyncAllApis();
                }
                
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}