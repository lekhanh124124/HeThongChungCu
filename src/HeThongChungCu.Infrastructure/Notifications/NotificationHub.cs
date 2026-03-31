using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HeThongChungCu.Infrastructure.Notifications;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            // Thêm vào nhóm cá nhân
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        if (!string.IsNullOrEmpty(role))
        {
            // Thêm vào nhóm theo vai trò (BQL, CuDan...)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role}");
        }

        await base.OnConnectedAsync();
    }
}
