using Microsoft.Build.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class RegCivilBackground : IHostedService
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IStringLocalizer _localizer;
        //private Timer _timer;
        public RegCivilBackground(IServiceProvider services,
            IStringLocalizer<RegCivilBackground> localizer,
            ILogger<RegCivilBackground> logger)
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
                        .GetRequiredService<IRegCivil>();

                var success = false;
                success = await scopedProcessingService.Init().ConfigureAwait(false);
                while (!success)
                {
                    await scopedProcessingService.CloseBW().ConfigureAwait(false);
                    success = await scopedProcessingService.Init().ConfigureAwait(false);
                }
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
                        .GetRequiredService<IRegCivil>();

                await scopedProcessingService.CloseBW().ConfigureAwait(false);
            }
        }
    }
}
