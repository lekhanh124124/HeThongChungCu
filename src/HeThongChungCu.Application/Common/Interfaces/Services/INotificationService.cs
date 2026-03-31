namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface INotificationService
{
    /// <summary>
    /// Đẩy thông báo thời gian thực tới danh sách người dùng qua WebSocket
    /// </summary>
    Task PushToUsersAsync(IEnumerable<int> userIds, object message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Đẩy thông báo thời gian thực tới một nhóm quyền qua WebSocket
    /// </summary>
    Task PushToRoleAsync(string roleName, object message, CancellationToken cancellationToken = default);
}
