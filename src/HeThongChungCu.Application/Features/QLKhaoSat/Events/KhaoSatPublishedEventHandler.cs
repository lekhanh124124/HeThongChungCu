using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Events;

public class KhaoSatPublishedEventHandler : INotificationHandler<KhaoSatPublishedEvent>
{
    private readonly ITaiKhoanCommandRepository _taiKhoanCommandRepository;
    private readonly IThongBaoCommandRepository _thongBaoCommandRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<KhaoSatPublishedEventHandler> _logger;

    public KhaoSatPublishedEventHandler(
        ITaiKhoanCommandRepository taiKhoanCommandRepository,
        IThongBaoCommandRepository thongBaoCommandRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<KhaoSatPublishedEventHandler> logger)
    {
        _taiKhoanCommandRepository = taiKhoanCommandRepository;
        _thongBaoCommandRepository = thongBaoCommandRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(KhaoSatPublishedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling KhaoSatPublishedEvent for KhaoSatId: {KhaoSatId}", notification.KhaoSatId);

            // 1. Lấy danh sách cư dân
            var residentIds = await _taiKhoanCommandRepository.GetNguoiDungIdsByRoleAsync(Role.Resident, cancellationToken);
            if (!residentIds.Any())
            {
                _logger.LogInformation("No residents found to notify for KhaoSatId: {KhaoSatId}", notification.KhaoSatId);
                return;
            }

            // 2. Tạo thông báo
            var tieuDeThongBao = $"Khảo sát mới: {notification.TieuDe}";
            var noiDungThongBao = "Ban quản lý vừa công bố một khảo sát mới. Vui lòng tham gia để đóng góp ý kiến!";
            
            var thongBao = new HeThongChungCu.Domain.Entities.ThongBao(
                tieuDeThongBao,
                noiDungThongBao,
                LoaiThongBao.KhaoSat,
                notification.KhaoSatId.ToString()
            );

            // 3. Phân bổ thông báo cho tất cả cư dân
            foreach (var userId in residentIds)
            {
                thongBao.ThemPhanBo(userId);
            }

            // 4. Lưu vào cơ sở dữ liệu
            await _thongBaoCommandRepository.AddAsync(thongBao, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Gửi thông báo realtime qua SignalR
            var notificationMessage = new
            {
                Id = thongBao.Id,
                TieuDe = thongBao.TieuDe,
                NoiDung = thongBao.NoiDung,
                LoaiThongBao = thongBao.LoaiThongBao.Value,
                ReferenceId = thongBao.ReferenceId,
                NgayTao = DateTimeOffset.UtcNow
            };

            await _notificationService.PushToUsersAsync(residentIds, notificationMessage, cancellationToken);
            
            _logger.LogInformation("Successfully sent and saved notifications for {Count} residents regarding KhaoSatId: {KhaoSatId}", residentIds.Count, notification.KhaoSatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling KhaoSatPublishedEvent for KhaoSatId: {KhaoSatId}", notification.KhaoSatId);
            throw;
        }
    }
}
