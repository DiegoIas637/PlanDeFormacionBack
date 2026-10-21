using Domain.Ports;
using Microsoft.Extensions.Options;

namespace PlanDeFormacionBack.BackgroundServices
{
    public class LowStockMonitorWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LowStockMonitorWorker> _logger;
        private readonly LowStockMonitorOptions _options;

        public LowStockMonitorWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<LowStockMonitorWorker> logger,
            IOptions<LowStockMonitorOptions> options)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = TimeSpan.FromSeconds(Math.Max(1, _options.IntervalSeconds));

            _logger.LogInformation(
                "LowStockMonitor started. Interval: {Interval}s, Threshold: {Threshold}.",
                interval.TotalSeconds,
                _options.Threshold);

            using var timer = new PeriodicTimer(interval);

            try
            {
                do
                {
                    await CheckLowStockAsync(stoppingToken);
                }
                while (await timer.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("LowStockMonitor is stopping due to cancellation.");
            }
        }

        private async Task CheckLowStockAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

                var lowStock = await productRepository.GetLowStockAsync(_options.Threshold, cancellationToken);

                if (lowStock.Count == 0)
                {
                    _logger.LogInformation("No products below the stock threshold of {Threshold}.", _options.Threshold);
                    return;
                }

                _logger.LogWarning(
                    "{Count} product(s) at or below the stock threshold of {Threshold}.",
                    lowStock.Count,
                    _options.Threshold);

                foreach (var product in lowStock)
                {
                    _logger.LogWarning(
                        "Low stock alert: '{Name}' (Id: {Id}) has {Stock} unit(s) left.",
                        product.Name,
                        product.Id,
                        product.Stock);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking low stock products.");
            }
        }
    }
}
