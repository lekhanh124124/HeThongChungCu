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

public class YeuCauThiCongCreatedEventHandler : INotificationHandler<YeuCauThiCongCreatedEvent>
{
    private readonly ITaiKhoanCommandRepository _taiKhoanRepository;
    private readonly IThongBaoCommandRepository _thongBaoRepository;
    private readonly IYeuCauThiCongQueryRepository _yeuCauQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<YeuCauThiCongCreatedEventHandler> _logger;

    public YeuCauThiCongCreatedEventHandler(
        ITaiKhoanCommandRepository taiKhoanRepository,
        IThongBaoCommandRepository thongBaoRepository,
        IYeuCauThiCongQueryRepository yeuCauQueryRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IDateTimeProvider dateTimeProvider,
        ILogger<YeuCauThiCongCreatedEventHandler> logger)
    {
        _taiKhoanRepository = taiKhoanRepository;
        _thongBaoRepository = thongBaoRepository;
        _yeuCauQueryRepository = yeuCauQueryRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task Handle(YeuCauThiCongCreatedEvent notification, CancellationToken cancellationToken)
    {
        var yeuCau = notification.YeuCauThiCong;
        _logger.LogInformation("Handling YeuCauThiCongCreatedEvent for Request ID: {Id}", yeuCau.Id);

        string title = "Yêu cầu thi công mới";
        string content = $"Căn hộ {yeuCau.CanHoId} đã gửi yêu cầu thi công mới: {yeuCau.HangMucThiCong}";
        var loaiThongBao = LoaiThongBao.YeuCauThiCong;

        // 1. Tìm người nhận thuộc BQL (Manager, Staff & Admin)
        var managerIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Manager, cancellationToken);
        var staffIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Staff, cancellationToken);
        var adminIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Admin, cancellationToken);

        var allRecipientIds = managerIds.Concat(staffIds).Concat(adminIds).Distinct().ToList();

        if (allRecipientIds.Count == 0) return;

        // 2. Lấy dữ liệu đầy đủ để đưa vào Metadata
        var detail = await _yeuCauQueryRepository.GetByIdAsync(new GetYeuCauThiCongByIdSpecification(yeuCau.Id), cancellationToken);
        string? metadataJson = detail != null ? JsonSerializer.Serialize(detail) : null;

        // 3. Tạo thực thể ThongBao và Phân bổ
        var thongBao = new HeThongChungCu.Domain.Entities.ThongBao(title, content, loaiThongBao, yeuCau.Id.ToString(), metadataJson);
        foreach (var recipientId in allRecipientIds)
        {
            thongBao.ThemPhanBo(recipientId);
        }

        // 4. Lưu vào Database
        await _thongBaoRepository.AddAsync(thongBao, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Đẩy thông báo thời gian thực
        await _notificationService.PushToUsersAsync(allRecipientIds, new
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
