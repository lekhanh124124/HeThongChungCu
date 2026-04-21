using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.QLCuTru.EventHandlers;

public class KetThucCuTruEventHandler : INotificationHandler<KetThucCuTruEvent>
{
    private readonly IYeuCauCuTruCommandRepository _yeuCauRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<KetThucCuTruEventHandler> _logger;

    public KetThucCuTruEventHandler(
        IYeuCauCuTruCommandRepository yeuCauRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork,
        ILogger<KetThucCuTruEventHandler> logger)
    {
        _yeuCauRepository = yeuCauRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(KetThucCuTruEvent notification, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId ?? 1;

        var processedAt = _dateTimeProvider.Now;
        _logger.LogInformation("Xử lý KetThucCuTruEvent cho căn hộ {CanHoId}, loại quan hệ: {LoaiQuanHe}",
            notification.CanHoId, notification.LoaiQuanHe.Name);

        // Chỉ xử lý nếu người rời đi là Chủ hộ
        if (notification.LoaiQuanHe != LoaiQuanHeCuTru.ChuHo)
        {
            return;
        }

        var filterStatuses = new[] { TrangThaiYeuCau.Pending, TrangThaiYeuCau.Saved };

        var pendingRequests = await _yeuCauRepository.GetByCanHoIdAndStatusesAsync(
            notification.CanHoId,
            filterStatuses,
            cancellationToken);

        var requestsList = pendingRequests.ToList();
        if (requestsList.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Tìm thấy {Count} yêu cầu cần hủy cho căn hộ {CanHoId}",
            requestsList.Count, notification.CanHoId);

        foreach (var yeuCau in requestsList)
        {
            yeuCau.Invalidate(adminId, "Chủ hộ bảo lãnh đã kết thúc cư trú.", processedAt);
            _yeuCauRepository.Update(yeuCau);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
