using HeThongChungCu.Domain.Events;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.Auth.EventHandlers;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event Received: UserRegisteredEvent triggered for Username {Username} (UserId: {UserId})", notification.Username, notification.UserId);

        return Task.CompletedTask;
    }
}
