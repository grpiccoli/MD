using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class FonasaBackground : IHostedService
    {
        //private int executionCount = 0;
        private readonly ILogger _logger;
        private readonly IStringLocalizer _localizer;
        //private Timer _timer;
        public FonasaBackground(
            IServiceProvider services,
            IStringLocalizer<FonasaBackground> localizer,
            ILogger<FonasaBackground> logger)
        {
            _localizer = localizer;
            Services = services;
            _logger = logger;
        }
        public IServiceProvider Services { get; }
        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                _localizer["Consume Scoped Service Hosted Service is working."]);

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IFonasa>();

                await scopedProcessingService.Init().ConfigureAwait(false);
            }
        }
        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                _localizer["Consume Scoped Service Hosted Service is stopping."]);

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IFonasa>();

                await scopedProcessingService.CloseBW().ConfigureAwait(false);
            }
        }
    }
}
