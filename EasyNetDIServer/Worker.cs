using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyNetDIServer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScope _serviceScope;
        
        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScope = serviceScopeFactory.CreateScope();
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, stoppingToken);

                }
                catch (Exception)
                {
                    //Do nothing shutting down
                }
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Async");

            var bus = _serviceScope.ServiceProvider.GetService<IBus>();
            while (bus != null && !bus.Advanced.IsConnected)
            {
                Thread.Sleep(1000);
                _logger.LogDebug("Waiting for Bus Connection");
            }

            return base.StartAsync(cancellationToken);
            
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Async");
            Dispose();
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _serviceScope?.Dispose();
            _logger.LogInformation("Dispose");
            base.Dispose();
        }
    }
}