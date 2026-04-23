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

public class YeuCauThiCongReturnedEventHandler : INotificationHandler<YeuCauThiCongReturnedEvent>
{
    private readonly IThongBaoCommandRepository _thongBaoRepository;
    private readonly IYeuCauThiCongQueryRepository _yeuCauQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<YeuCauThiCongReturnedEventHandler> _logger;

    public YeuCauThiCongReturnedEventHandler(
        IThongBaoCommandRepository thongBaoRepository,
        IYeuCauThiCongQueryRepository yeuCauQueryRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IDateTimeProvider dateTimeProvider,
        ILogger<YeuCauThiCongReturnedEventHandler> logger)
    {
        _thongBaoRepository = thongBaoRepository;
        _yeuCauQueryRepository = yeuCauQueryRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task Handle(YeuCauThiCongReturnedEvent notification, CancellationToken cancellationToken)
    {
        var yeuCau = notification.YeuCauThiCong;
        _logger.LogInformation("Handling YeuCauThiCongReturnedEvent for Request ID: {Id}", yeuCau.Id);

        string title = "Yêu cầu thi công bị trả lại";
        string content = $"Yêu cầu thi công {yeuCau.HangMucThiCong} cần bổ sung thông tin. Lý do: {yeuCau.LyDo}";
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
