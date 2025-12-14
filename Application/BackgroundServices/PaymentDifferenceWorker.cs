// Infrastructure/BackgroundServices/PaymentDifferenceWorker.cs

using Application.BackgroundJobs;
using Application.Refits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Application.BackgroundServices;

public class PaymentDifferenceWorker : BackgroundService
{
    private readonly Channel<PaymentDifferenceJob> _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PaymentDifferenceWorker> _logger;

    public PaymentDifferenceWorker(
        Channel<PaymentDifferenceJob> channel,
        IServiceProvider serviceProvider,
        ILogger<PaymentDifferenceWorker> logger)
    {
        _channel = channel;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = Enumerable.Range(0, 5)
            .Select(_ => Task.Run(() => ProcessQueueAsync(stoppingToken), stoppingToken))
            .ToArray();

        await Task.WhenAll(tasks);
    }

    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        await foreach (var job in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            await ProcessSingleJobAsync(job, cancellationToken);
        }
    }

    private async Task ProcessSingleJobAsync(PaymentDifferenceJob job, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var paymentOrg = scope.ServiceProvider.GetRequiredService<IPaymentOrganization>();

        try
        {
            var response = await paymentOrg.CreateCaseDifference(
                  token: $"token={job.SystemToken}",job.Data.ApplicationName,job.Data.LicenseType,job.Data.WfClass,job.Data.NationalCode);

            if (response.Data.Status == Bizagi.Application.Models.ActionResultStatus.Complete)
            {
                _logger.LogInformation("PaymentDifference succeeded for NationalCode {NationalCode} | RequestId: {RequestId}",
                    job.Data.NationalCode, job.RequestId);
            }
           
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in PaymentDifference for NationalCode {NationalCode} | RequestId: {RequestId}",
                job.Data.NationalCode, job.RequestId);
        }

        // کمی تاخیر برای جلوگیری از Rate Limit
        await Task.Delay(150, ct);
    }
}