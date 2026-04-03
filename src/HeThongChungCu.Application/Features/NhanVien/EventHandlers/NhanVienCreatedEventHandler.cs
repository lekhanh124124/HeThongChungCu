using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.NhanVien.EventHandlers;

public class NhanVienCreatedEventHandler : INotificationHandler<NhanVienCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<NhanVienCreatedEventHandler> _logger;

    public NhanVienCreatedEventHandler(
        IEmailService emailService,
        ILogger<NhanVienCreatedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(NhanVienCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing staff welcome email for: {Email}", notification.Email);

        try
        {
            await _emailService.SendStaffWelcomeEmailAsync(
                notification.Email,
                notification.FullName,
                notification.UserName,
                notification.Password,
                cancellationToken);

            _logger.LogInformation("Successfully sent welcome email to staff: {Email}", notification.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to staff: {Email}. Error: {Message}", 
                notification.Email, ex.Message);
            
            // Note: We don't throw here to avoid failing the main transaction if email sending fails,
            // as the staff record and account are already created. In a production system, 
            // the Outbox pattern or a retry mechanism would be used for reliability.
        }
    }
}
