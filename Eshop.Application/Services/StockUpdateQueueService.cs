using Eshop.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Eshop.Application.Services
{
    public class StockUpdateQueueService : IStockUpdateQueueService
    {
        private readonly Channel<UpdateStockTask> _queue;

        public StockUpdateQueueService()
        {
            // Bounded channel helps prevent memory issues if the queue grows too fast
            var options = new BoundedChannelOptions(1000) { FullMode = BoundedChannelFullMode.Wait };
            _queue = Channel.CreateBounded<UpdateStockTask>(options);
        }

        public async ValueTask QueueStockUpdateAsync(UpdateStockTask task)
        {
            await _queue.Writer.WriteAsync(task);
        }

        public async ValueTask<UpdateStockTask> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
