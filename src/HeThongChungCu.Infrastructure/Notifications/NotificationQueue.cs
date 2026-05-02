using System.Threading.Channels;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Infrastructure.Notifications;

public class NotificationQueue : INotificationQueue
{
    private readonly Channel<PaymentNotificationRequest> _queue;

    public NotificationQueue()
    {
        // Unbounded channel để tránh chặn producer, tùy vào bộ nhớ có thể dùng Bounded
        _queue = Channel.CreateUnbounded<PaymentNotificationRequest>();
    }

    public async ValueTask EnqueueAsync(PaymentNotificationRequest request)
    {
        await _queue.Writer.WriteAsync(request);
    }

    public async ValueTask<PaymentNotificationRequest> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
