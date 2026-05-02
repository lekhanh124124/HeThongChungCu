using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.QLThanhToan.EventHandlers;

public class DotThanhToanPhatHanhEventHandler : INotificationHandler<DotThanhToanPhatHanhEvent>
{
    private readonly IQuanHeCuTruCommandRepository _cuTruRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ITaiKhoanCommandRepository _taiKhoanRepository;
    private readonly INotificationQueue _notificationQueue;
    private readonly ILogger<DotThanhToanPhatHanhEventHandler> _logger;

    public DotThanhToanPhatHanhEventHandler(
        IQuanHeCuTruCommandRepository cuTruRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        ITaiKhoanCommandRepository taiKhoanRepository,
        INotificationQueue notificationQueue,
        ILogger<DotThanhToanPhatHanhEventHandler> logger)
    {
        _cuTruRepository = cuTruRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _taiKhoanRepository = taiKhoanRepository;
        _notificationQueue = notificationQueue;
        _logger = logger;
    }

    public async Task Handle(DotThanhToanPhatHanhEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing notifications for DotThanhToan: {DotName}", notification.DotThanhToan.TenDot);

        var invoicesByCanHo = notification.HoaDons.GroupBy(x => x.CanHoId).ToList();
        var canHoIds = invoicesByCanHo.Select(x => x.Key).Distinct().ToList();

        // 1. Batch load all residents for these apartments
        var allResidents = await _cuTruRepository.GetByCanHoIdsAsync(canHoIds, cancellationToken);
        var representatives = allResidents
            .Where(r => (r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue) && 
                        r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru)
            .GroupBy(r => r.CanHoId)
            .ToDictionary(g => g.Key, g => g.First());

        var representativeUserIds = representatives.Values.Select(r => r.NguoiDungId).Distinct().ToList();

        // 2. Batch load all users
        var users = await _nguoiDungRepository.GetByIdsAsync(representativeUserIds, cancellationToken);
        var userLookup = users.ToDictionary(u => u.Id);

        // 3. Batch load all accounts
        var accounts = await _taiKhoanRepository.GetByNguoiDungIdsAsync(representativeUserIds, cancellationToken);
        var accountLookup = accounts
            .Where(a => a.NguoiDungId.HasValue)
            .ToDictionary(a => a.NguoiDungId!.Value);

        foreach (var group in invoicesByCanHo)
        {
            var canHoId = group.Key;
            var hoaDons = group.ToList();

            if (!representatives.TryGetValue(canHoId, out var representative))
            {
                _logger.LogWarning("No representative found for CanHoId: {CanHoId}. Skipping notification.", canHoId);
                continue;
            }

            if (!userLookup.TryGetValue(representative.NguoiDungId, out var user))
            {
                _logger.LogWarning("User not found for NguoiDungId: {NguoiDungId}. Skipping notification.", representative.NguoiDungId);
                continue;
            }

            accountLookup.TryGetValue(user.Id, out var account);
            string? email = account?.Email?.Value;

            // Đẩy vào hàng đợi để xử lý background
            await _notificationQueue.EnqueueAsync(new PaymentNotificationRequest
            {
                DotThanhToan = notification.DotThanhToan,
                HoaDons = hoaDons,
                User = user,
                Email = email
            });
        }

        _logger.LogInformation("Finished enqueuing notifications for DotThanhToan: {DotName}", notification.DotThanhToan.TenDot);
    }
}
