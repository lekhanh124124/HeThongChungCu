using System.Text.Json;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.EventHandlers;

public class YeuCauSuaChuaHoanTatEventHandler : INotificationHandler<YeuCauSuaChuaHoanTatEvent>
{
    private readonly IThongBaoCommandRepository _thongBaoRepository;
    private readonly IYeuCauSuaChuaQueryRepository _yeuCauQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<YeuCauSuaChuaHoanTatEventHandler> _logger;

    public YeuCauSuaChuaHoanTatEventHandler(
        IThongBaoCommandRepository thongBaoRepository,
        IYeuCauSuaChuaQueryRepository yeuCauQueryRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IDateTimeProvider dateTimeProvider,
        ILogger<YeuCauSuaChuaHoanTatEventHandler> logger)
    {
        _thongBaoRepository = thongBaoRepository;
        _yeuCauQueryRepository = yeuCauQueryRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task Handle(YeuCauSuaChuaHoanTatEvent notification, CancellationToken cancellationToken)
    {
        var yeuCau = notification.YeuCauSuaChua;
        _logger.LogInformation("Handling YeuCauSuaChuaHoanTatEvent for Request ID: {Id}", yeuCau.Id);

        string title = "Sửa chữa hoàn tất";
        string content = $"Yêu cầu sửa chữa {yeuCau.NoiDung} đã hoàn tất. Chi phí quyết toán: {yeuCau.ChiPhiThucTe?.ToString("N0")} VNĐ.";
        var loaiThongBao = LoaiThongBao.YeuCauSuaChua;

        var recipientIds = new List<int> { yeuCau.CreatedBy };

        // 1. Lấy dữ liệu đầy đủ để đưa vào Metadata
        var detail = await _yeuCauQueryRepository.GetByIdAsync(new GetYeuCauSuaChuaByIdSpecification(yeuCau.Id), cancellationToken);
        string? metadataJson = detail != null ? JsonSerializer.Serialize(detail) : null;

        // 2. Tạo thực thể ThongBao và Phân bổ
        var thongBao = new HeThongChungCu.Domain.Entities.ThongBao(title, content, loaiThongBao, yeuCau.Id.ToString(), metadataJson);
        foreach (var recipientId in recipientIds)
        {
            thongBao.ThemPhanBo(recipientId);
        }

        // 3. Lưu vào Database
        await _thongBaoRepository.AddAsync(thongBao, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Đẩy thông báo thời gian thực
        await _notificationService.PushToUsersAsync(recipientIds, new
        {
            Id = thongBao.Id,
            TieuDe = title,
            NoiDung = content,
            LoaiThongBaoId = loaiThongBao.Value,
            TenLoaiThongBao = loaiThongBao.Name,
            ReferenceId = yeuCau.Id.ToString(),
            Metadata = detail,
            CreatedAt = _dateTimeProvider.Now
        }, cancellationToken);
    }
}
