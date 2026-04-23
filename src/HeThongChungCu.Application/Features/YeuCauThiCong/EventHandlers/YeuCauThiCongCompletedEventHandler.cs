using System.Text.Json;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.EventHandlers;

public class YeuCauThiCongCompletedEventHandler : INotificationHandler<YeuCauThiCongCompletedEvent>
{
    private readonly IThongBaoCommandRepository _thongBaoRepository;
    private readonly IYeuCauThiCongQueryRepository _yeuCauQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<YeuCauThiCongCompletedEventHandler> _logger;

    public YeuCauThiCongCompletedEventHandler(
        IThongBaoCommandRepository thongBaoRepository,
        IYeuCauThiCongQueryRepository yeuCauQueryRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IDateTimeProvider dateTimeProvider,
        ILogger<YeuCauThiCongCompletedEventHandler> logger)
    {
        _thongBaoRepository = thongBaoRepository;
        _yeuCauQueryRepository = yeuCauQueryRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task Handle(YeuCauThiCongCompletedEvent notification, CancellationToken cancellationToken)
    {
        var yeuCau = notification.YeuCauThiCong;
        _logger.LogInformation("Handling YeuCauThiCongCompletedEvent for Request ID: {Id}", yeuCau.Id);

        string title = "Đóng hồ sơ thi công";
        string content = $"Hồ sơ thi công {yeuCau.HangMucThiCong} đã chính thức đóng.";
        var loaiThongBao = LoaiThongBao.YeuCauThiCong;

        var recipientIds = new List<int> { yeuCau.CreatedBy };

        // 1. Lấy dữ liệu đầy đủ để đưa vào Metadata
        var detail = await _yeuCauQueryRepository.GetByIdAsync(new GetYeuCauThiCongByIdSpecification(yeuCau.Id), cancellationToken);
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
