using HeThongChungCu.Application.Features.QLThanhToan.Commands.CreateDotThanhToan;
using HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetLatestOpenDotThanhToan;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Services;

public class MonthlyBillingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MonthlyBillingBackgroundService> _logger;

    public MonthlyBillingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<MonthlyBillingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MonthlyBillingBackgroundService is starting...");

        // Chờ một khoảng thời gian ngắn sau khi startup để đảm bảo hệ thống ổn định
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Checking and generating monthly draft invoices...");
                await GenerateInvoicesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during monthly billing generation.");
            }

            // Chạy định kỳ mỗi 24 giờ (Có thể tinh chỉnh lại tùy nhu cầu)
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        _logger.LogInformation("MonthlyBillingBackgroundService is stopping.");
    }

    private async Task GenerateInvoicesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        var now = DateTimeOffset.Now;

        // 1. Kiểm tra xem đợt thanh toán đã tồn tại chưa qua Mediator
        var query = new GetLatestOpenDotThanhToanQuery(now.Month, now.Year);
        var queryResult = await mediator.Send(query, cancellationToken);

        int dotId;

        if (queryResult.IsFailure)
        {
            // Nếu không tìm thấy, tiến hành tạo mới
            _logger.LogInformation("No open payment period found for {Month}/{Year}. Creating new one...", now.Month, now.Year);
            var createResult = await mediator.Send(new CreateDotThanhToanCommand
            {
                Thang = now.Month,
                Nam = now.Year
            }, cancellationToken);

            if (createResult.IsFailure)
            {
                _logger.LogWarning("Failed to create payment period: {Error}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return;
            }
            dotId = createResult.Value.Id;
        }
        else
        {
            dotId = queryResult.Value.Id;
        }

        // 2. Chạy lệnh lập hóa đơn dự thảo
        var command = new LapHoaDonDuThaoCommand
        {
            DotThanhToanId = dotId
        };

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            if (result.Value.SoLuongHoaDonTaoMoi > 0)
            {
                _logger.LogInformation("Successfully processed monthly billing. New invoices created: {Count} for period {Month}/{Year}",
                    result.Value.SoLuongHoaDonTaoMoi, now.Month, now.Year);
            }
            else
            {
                _logger.LogInformation("Monthly billing checked. No new invoices needed for period {Month}/{Year}", now.Month, now.Year);
            }
        }
        else
        {
            _logger.LogWarning("Monthly billing process finished with issues: {Error}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
