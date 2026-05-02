using HeThongChungCu.Application.Common.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Services;

public class NotificationBackgroundService : BackgroundService
{
    private readonly INotificationQueue _notificationQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationBackgroundService> _logger;

    public NotificationBackgroundService(
        INotificationQueue notificationQueue,
        IServiceProvider serviceProvider,
        ILogger<NotificationBackgroundService> logger)
    {
        _notificationQueue = notificationQueue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NotificationBackgroundService is starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = await _notificationQueue.DequeueAsync(stoppingToken);

                // Sử dụng Scope để lấy các service Scoped (Email, Notification)
                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // 1. Gửi thông báo thời gian thực qua WebSocket
                var notifyMessage = new
                {
                    Title = "Thông báo phát hành hóa đơn",
                    Content = $"Đợt thanh toán {request.DotThanhToan.TenDot} đã được phát hành. Bạn có {request.HoaDons.Count} hóa đơn mới.",
                    Type = "BILLING",
                    DotThanhToanId = request.DotThanhToan.Id
                };
                
                await notificationService.PushToUsersAsync([request.User.Id], notifyMessage, stoppingToken);

                // 2. Gửi Email (Nếu có email)
                if (!string.IsNullOrEmpty(request.Email))
                {
                    var totalAmount = request.HoaDons.Sum(x => x.TongTien);
                    var emailSubject = $"[Thông báo] Phát hành hóa đơn đợt {request.DotThanhToan.TenDot}";
                    var emailBody = $@"
                        <h3>Kính chào ông/bà {request.User.HoTen},</h3>
                        <p>Ban quản lý chung cư xin thông báo đã phát hành hóa đơn cho đợt thanh toán: <b>{request.DotThanhToan.TenDot}</b>.</p>
                        <p>Chi tiết các hóa đơn:</p>
                        <ul>
                            {string.Join("", request.HoaDons.Select(h => $"<li>{h.MaHoaDon}: {h.TongTien:N0} VNĐ (Hạn: {h.NgayHanThanhToan:dd/MM/yyyy})</li>"))}
                        </ul>
                        <p><b>Tổng số tiền cần thanh toán: {totalAmount:N0} VNĐ</b></p>
                        <p>Vui lòng đăng nhập vào ứng dụng để xem chi tiết và thanh toán.</p>
                        <p>Trân trọng,<br/>Ban Quản Lý.</p>";

                    await emailService.SendAsync(request.Email, emailSubject, emailBody, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Ngắt khi shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing notification from queue.");
            }
        }

        _logger.LogInformation("NotificationBackgroundService is stopping.");
    }
}
