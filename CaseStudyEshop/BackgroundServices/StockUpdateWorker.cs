using Eshop.Application.DTO.Request;
using Eshop.Application.Interfaces;

namespace Eshop.API.BackgroundServices
{
    /// <summary>
    /// Background worker that processes stock updates from the queue.
    /// </summary>
    public class StockUpdateWorker : BackgroundService
    {
        private readonly IStockUpdateQueueService _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StockUpdateWorker> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockUpdateWorker"/> class.
        /// </summary>
        /// <param name="queue">The background queue service containing pending stock updates.</param>
        /// <param name="serviceProvider">The service provider used to create scopes for resolving scoped services.</param>
        /// <param name="logger">The logger for tracking background processing events and errors.</param>
        public StockUpdateWorker(IStockUpdateQueueService queue, IServiceProvider serviceProvider, ILogger<StockUpdateWorker> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Core loop that continuously monitors the queue and processes stock updates as they arrive.
        /// </summary>
        /// <param name="stoppingToken">Triggered when the application host is performing a graceful shutdown.</param>
        /// <returns>A <see cref="Task"/> representing the background operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // The worker runs in a loop until the application is stopped
            while (!stoppingToken.IsCancellationRequested)
            {
                // DequeueAsync will wait (pause) here without blocking the thread until a task is available
                var task = await _queue.DequeueAsync(stoppingToken);

                try
                {
                    // BackgroundService is a Singleton, so we must create a scope 
                    // to resolve Scoped services like IProductService (and its DB context)
                    using var scope = _serviceProvider.CreateScope();
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                    // Execute the actual business logic to update the database
                    await productService.UpdateStockAsync(task.ProductId, new UpdateStockRequestDto { NewQuantity = task.NewQuantity });

                    _logger.LogInformation("Background stock update successful for {ProductId}", task.ProductId);
                }
                catch (Exception ex)
                {
                    // Logs the error but keeps the loop running so other tasks can be processed
                    _logger.LogError(ex, "Error processing stock update for {ProductId}", task.ProductId);
                }
            }
        }
    }
}
