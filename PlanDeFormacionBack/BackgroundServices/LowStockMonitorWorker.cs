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
                "Monitor de stock bajo iniciado. Intervalo: {Interval}s, Umbral: {Threshold}.",
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
                _logger.LogInformation("El monitor de stock bajo se está deteniendo.");
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
                    _logger.LogInformation("No hay productos por debajo del umbral de stock de {Threshold}.", _options.Threshold);
                    return;
                }

                _logger.LogWarning(
                    "{Count} producto(s) en o por debajo del umbral de stock de {Threshold}.",
                    lowStock.Count,
                    _options.Threshold);

                foreach (var product in lowStock)
                {
                    _logger.LogWarning(
                        "Alerta de stock bajo: '{Name}' (Id: {Id}) tiene {Stock} unidad(es) restante(s).",
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
                _logger.LogError(ex, "Ocurrió un error al revisar los productos con stock bajo.");
            }
        }
    }
}
