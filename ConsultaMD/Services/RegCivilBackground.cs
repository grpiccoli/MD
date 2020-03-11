using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    //public class RegCivilBackground : IHostedService
    //{
    //    private readonly ILogger _logger;
    //    private readonly IStringLocalizer _localizer;
    //    public RegCivilBackground(
    //        IServiceProvider services,
    //        IStringLocalizer<RegCivilBackground> localizer,
    //        ILogger<RegCivilBackground> logger)
    //    {
    //        _localizer = localizer;
    //        Services = services;
    //        _logger = logger;
    //    }
    //    public IServiceProvider Services { get; }
    //    public async Task StartAsync(CancellationToken stoppingToken)
    //    {
    //        _logger.LogInformation(
    //            _localizer["Consume Scoped Service Hosted Service is working."]);

    //        using var scope = Services.CreateScope();
    //        var scopedProcessingService =
    //            scope.ServiceProvider
    //            .GetRequiredService<IRegCivil>();
    //        //await scopedProcessingService.Init().ConfigureAwait(false);

    //    }
    //    // noop
    //    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    //}
}
