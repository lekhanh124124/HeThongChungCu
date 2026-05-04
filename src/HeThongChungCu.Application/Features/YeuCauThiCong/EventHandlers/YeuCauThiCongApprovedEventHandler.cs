using System.Text.Json;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.EventHandlers;

public class YeuCauThiCongApprovedEventHandler : INotificationHandler<YeuCauThiCongApprovedEvent>
{
    private readonly IThongBaoCommandRepository _thongBaoRepository;
    private readonly IYeuCauThiCongQueryRepository _yeuCauQueryRepository;
    private readonly IYeuCauThiCongCommandRepository _yeuCauCommandRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IHoaDonCommandRepository _hoaDonRepository;
    private readonly IBillingDomainService _billingDomainService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<YeuCauThiCongApprovedEventHandler> _logger;

    public YeuCauThiCongApprovedEventHandler(
        IThongBaoCommandRepository thongBaoRepository,
        IYeuCauThiCongQueryRepository yeuCauQueryRepository,
        IYeuCauThiCongCommandRepository yeuCauCommandRepository,
        ICanHoCommandRepository canHoRepository,
        IHoaDonCommandRepository hoaDonRepository,
        IBillingDomainService billingDomainService,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IDateTimeProvider dateTimeProvider,
        ILogger<YeuCauThiCongApprovedEventHandler> logger)
    {
        _thongBaoRepository = thongBaoRepository;
        _yeuCauQueryRepository = yeuCauQueryRepository;
        _yeuCauCommandRepository = yeuCauCommandRepository;
        _canHoRepository = canHoRepository;
        _hoaDonRepository = hoaDonRepository;
        _billingDomainService = billingDomainService;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task Handle(YeuCauThiCongApprovedEvent notification, CancellationToken cancellationToken)
    {
        var yeuCau = notification.YeuCauThiCong;
        _logger.LogInformation("Handling YeuCauThiCongApprovedEvent for Request ID: {Id}", yeuCau.Id);

        // 1. Tạo hóa đơn tiền đặt cọc để cư dân thực hiện thanh toán
        await TryCreateDepositInvoiceAsync(yeuCau, cancellationToken);

        // 2. Lấy dữ liệu đầy đủ để đưa vào Metadata thông báo
        var detail = await _yeuCauQueryRepository.GetByIdAsync(new GetYeuCauThiCongByIdSpecification(yeuCau.Id), cancellationToken);
        string? metadataJson = detail != null ? JsonSerializer.Serialize(detail) : null;

        // 3. Tạo và gửi thông báo cho cư dân
        string title = "Yêu cầu thi công đã được duyệt";
        string content = $"Yêu cầu \"{yeuCau.HangMucThiCong}\" đã được duyệt. Vui lòng thực hiện đặt cọc {yeuCau.TienDatCoc?.ToString("N0")} VNĐ để bắt đầu thi công.";
        var loaiThongBao = LoaiThongBao.YeuCauThiCong;
        var recipientIds = new List<int> { yeuCau.CreatedBy };

        var thongBao = new HeThongChungCu.Domain.Entities.ThongBao(title, content, loaiThongBao, yeuCau.Id.ToString(), metadataJson);
        foreach (var recipientId in recipientIds)
        {
            thongBao.ThemPhanBo(recipientId);
        }

        await _thongBaoRepository.AddAsync(thongBao, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

    private async Task TryCreateDepositInvoiceAsync(Domain.Entities.YeuCauThiCong yeuCau, CancellationToken cancellationToken)
    {
        if ((yeuCau.TienDatCoc ?? 0) <= 0)
        {
            _logger.LogInformation("YeuCauThiCong {Id}: Không có tiền cọc — bỏ qua tạo hóa đơn.", yeuCau.Id);
            return;
        }

        var canHo = await _canHoRepository.GetByIdAsync(yeuCau.CanHoId, cancellationToken);
        if (canHo == null)
        {
            _logger.LogWarning("YeuCauThiCong {Id}: Không tìm thấy căn hộ {CanHoId} — bỏ qua tạo hóa đơn.", yeuCau.Id, yeuCau.CanHoId);
            return;
        }

        // HD-TC-{MaCanHo}-{YeuCauId}: duy nhất và có thể trace ngược
        string maHoaDon = $"HD-TC-{canHo.MaCanHo}-{yeuCau.Id}";
        // Hạn nộp cọc 7 ngày kể từ ngày duyệt
        var ngayHan = _dateTimeProvider.Now.AddDays(7);

        var hoaDonResult = _billingDomainService.CreateInvoiceForConstruction(yeuCau, canHo, maHoaDon, ngayHan);
        if (hoaDonResult.IsFailure)
        {
            _logger.LogWarning("YeuCauThiCong {Id}: Tạo hóa đơn cọc thất bại — {Error}.", yeuCau.Id, hoaDonResult.Errors.FirstOrDefault()?.Description);
            return;
        }

        await _hoaDonRepository.AddAsync(hoaDonResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Ghi nhận HoaDonId ngược vào yêu cầu để có thể tra cứu
        yeuCau.MarkAsBilled(hoaDonResult.Value.Id);
        _yeuCauCommandRepository.Update(yeuCau);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("YeuCauThiCong {Id}: Đã tạo hóa đơn cọc {MaHoaDon} thành công.", yeuCau.Id, maHoaDon);
    }
}
