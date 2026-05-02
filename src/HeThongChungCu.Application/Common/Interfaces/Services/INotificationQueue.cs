using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface INotificationQueue
{
    ValueTask EnqueueAsync(PaymentNotificationRequest request);
    ValueTask<PaymentNotificationRequest> DequeueAsync(CancellationToken cancellationToken);
}
