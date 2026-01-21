using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace Eshop.Application.Interfaces
{


    /// <summary>
    /// Provides a thread-safe queue for stock update background tasks.
    /// </summary>
    public interface IStockUpdateQueueService
    {
        ValueTask QueueStockUpdateAsync(UpdateStockTask task);
        ValueTask<UpdateStockTask> DequeueAsync(CancellationToken cancellationToken);
    }

    public record UpdateStockTask(Guid ProductId, int NewQuantity);
}
