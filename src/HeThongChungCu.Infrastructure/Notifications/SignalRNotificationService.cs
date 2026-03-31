using HeThongChungCu.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace HeThongChungCu.Infrastructure.Notifications;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PushToUsersAsync(IEnumerable<int> userIds, object message, CancellationToken cancellationToken = default)
    {
        foreach (var userId in userIds)
        {
            await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", message, cancellationToken);
        }
    }

    public async Task PushToRoleAsync(string roleName, object message, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"Role_{roleName}").SendAsync("ReceiveNotification", message, cancellationToken);
    }
}
