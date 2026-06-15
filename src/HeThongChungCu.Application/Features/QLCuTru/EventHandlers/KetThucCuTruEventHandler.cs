using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.QLCuTru.EventHandlers;

public class KetThucCuTruEventHandler : INotificationHandler<KetThucCuTruEvent>
{
    private readonly IYeuCauCuTruCommandRepository _yeuCauCuTruRepository;
    private readonly IYeuCauSuaChuaCommandRepository _yeuCauSuaChuaRepository;
    private readonly IYeuCauThiCongCommandRepository _yeuCauThiCongRepository;
    private readonly IYeuCauPhanAnhCommandRepository _yeuCauPhanAnhRepository;
    private readonly IYeuCauPhuongTienCommandRepository _yeuCauPhuongTienRepository;
    private readonly IPhuongTienCommandRepository _phuongTienRepository;
    private readonly IDangKyDichVuCommandRepository _dangKyDichVuRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<KetThucCuTruEventHandler> _logger;

    public KetThucCuTruEventHandler(
        IYeuCauCuTruCommandRepository yeuCauCuTruRepository,
        IYeuCauSuaChuaCommandRepository yeuCauSuaChuaRepository,
        IYeuCauThiCongCommandRepository yeuCauThiCongRepository,
        IYeuCauPhanAnhCommandRepository yeuCauPhanAnhRepository,
        IYeuCauPhuongTienCommandRepository yeuCauPhuongTienRepository,
        IPhuongTienCommandRepository phuongTienRepository,
        IDangKyDichVuCommandRepository dangKyDichVuRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork,
        ILogger<KetThucCuTruEventHandler> logger)
    {
        _yeuCauCuTruRepository = yeuCauCuTruRepository;
        _yeuCauSuaChuaRepository = yeuCauSuaChuaRepository;
        _yeuCauThiCongRepository = yeuCauThiCongRepository;
        _yeuCauPhanAnhRepository = yeuCauPhanAnhRepository;
        _yeuCauPhuongTienRepository = yeuCauPhuongTienRepository;
        _phuongTienRepository = phuongTienRepository;
        _dangKyDichVuRepository = dangKyDichVuRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(KetThucCuTruEvent notification, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId ?? 1;
        var processedAt = _dateTimeProvider.Now;
        var now = processedAt.DateTime;

        _logger.LogInformation("Xử lý KetThucCuTruEvent cho căn hộ {CanHoId}, loại quan hệ: {LoaiQuanHe}, userId: {UserId}",
            notification.CanHoId, notification.LoaiQuanHe.Name, notification.NguoiDungId);

        // Chỉ xử lý nghiệp vụ chung cho toàn căn hộ nếu người dọn đi là Chủ hộ hoặc Người thuê chính
        if (notification.LoaiQuanHe != LoaiQuanHeCuTru.ChuHo && notification.LoaiQuanHe != LoaiQuanHeCuTru.NguoiThue)
        {
            return;
        }

        var filterStatuses = new[] { TrangThaiYeuCau.Pending, TrangThaiYeuCau.Saved };
        var reason = "Chủ hộ/Người thuê chính đã kết thúc cư trú.";

        // 1. Hủy các yêu cầu chờ xử lý
        var yeuCauCuTrus = await _yeuCauCuTruRepository.GetByCanHoIdAndStatusesAsync(notification.CanHoId, filterStatuses, cancellationToken);
        foreach (var yc in yeuCauCuTrus)
        {
            yc.Invalidate(adminId, reason, processedAt);
            _yeuCauCuTruRepository.Update(yc);
        }

        var yeuCauSuaChuas = await _yeuCauSuaChuaRepository.GetByCanHoIdAndStatusesAsync(notification.CanHoId, filterStatuses, cancellationToken);
        foreach (var yc in yeuCauSuaChuas)
        {
            yc.Invalidate(adminId, reason, processedAt);
            _yeuCauSuaChuaRepository.Update(yc);
        }

        var yeuCauThiCongs = await _yeuCauThiCongRepository.GetByCanHoIdAndStatusesAsync(notification.CanHoId, filterStatuses, cancellationToken);
        foreach (var yc in yeuCauThiCongs)
        {
            yc.Invalidate(adminId, reason, processedAt);
            _yeuCauThiCongRepository.Update(yc);
        }

        var yeuCauPhanAnhs = await _yeuCauPhanAnhRepository.GetByCanHoIdAndStatusesAsync(notification.CanHoId, filterStatuses, cancellationToken);
        foreach (var yc in yeuCauPhanAnhs)
        {
            yc.Invalidate(adminId, reason, processedAt);
            _yeuCauPhanAnhRepository.Update(yc);
        }

        var yeuCauPhuongTiens = await _yeuCauPhuongTienRepository.GetByCanHoIdAndStatusesAsync(notification.CanHoId, filterStatuses, cancellationToken);
        foreach (var yc in yeuCauPhuongTiens)
        {
            yc.Invalidate(adminId, reason, processedAt);
            _yeuCauPhuongTienRepository.Update(yc);
        }

        // 2. Hủy các đăng ký dịch vụ đang chờ duyệt hoặc đang sử dụng (vì đã dọn đi)
        var dangKys = await _dangKyDichVuRepository.GetActiveSubscriptionsByCanHoAsync(notification.CanHoId, cancellationToken);
        foreach (var dk in dangKys)
        {
            dk.HuyDangKy(now);
            _dangKyDichVuRepository.Update(dk);
        }

        // 3. Vô hiệu hóa tất cả phương tiện của căn hộ (Khóa thẻ xe, hủy kích hoạt)
        var phuongTiens = await _phuongTienRepository.GetActiveByCanHoIdsAsync(new[] { notification.CanHoId }, cancellationToken);
        foreach (var pt in phuongTiens)
        {
            pt.Huy(now);
            _phuongTienRepository.Update(pt);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
