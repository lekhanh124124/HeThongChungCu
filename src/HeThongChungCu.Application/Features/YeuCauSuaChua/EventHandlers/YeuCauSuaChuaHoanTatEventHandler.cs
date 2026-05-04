using System.Text.Json;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.EventHandlers;

public class YeuCauSuaChuaHoanTatEventHandler : INotificationHandler<YeuCauSuaChuaHoanTatEvent>
{
    private readonly IThongBaoCommandRepository _thongBaoRepository;
    private readonly IYeuCauSuaChuaQueryRepository _yeuCauQueryRepository;
    private readonly IYeuCauSuaChuaCommandRepository _yeuCauCommandRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IHoaDonCommandRepository _hoaDonRepository;
    private readonly IBillingDomainService _billingDomainService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<YeuCauSuaChuaHoanTatEventHandler> _logger;

    public YeuCauSuaChuaHoanTatEventHandler(
        IThongBaoCommandRepository thongBaoRepository,
        IYeuCauSuaChuaQueryRepository yeuCauQueryRepository,
        IYeuCauSuaChuaCommandRepository yeuCauCommandRepository,
        ICanHoCommandRepository canHoRepository,
        IHoaDonCommandRepository hoaDonRepository,
        IBillingDomainService billingDomainService,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IDateTimeProvider dateTimeProvider,
        ILogger<YeuCauSuaChuaHoanTatEventHandler> logger)
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

    public async Task Handle(YeuCauSuaChuaHoanTatEvent notification, CancellationToken cancellationToken)
    {
        var yeuCau = notification.YeuCauSuaChua;
        _logger.LogInformation("Handling YeuCauSuaChuaHoanTatEvent for Request ID: {Id}", yeuCau.Id);

        // 1. Tạo hóa đơn nếu có phí thực tế (Post-paid, không thuộc Đợt thanh toán)
        await TryCreateRepairInvoiceAsync(yeuCau, cancellationToken);

        // 2. Lấy dữ liệu đầy đủ để đưa vào Metadata thông báo
        var detail = await _yeuCauQueryRepository.GetByIdAsync(new GetYeuCauSuaChuaByIdSpecification(yeuCau.Id), cancellationToken);
        string? metadataJson = detail != null ? JsonSerializer.Serialize(detail) : null;

        // 3. Tạo và gửi thông báo cho cư dân
        string title = "Sửa chữa hoàn tất";
        string content = yeuCau.IsMienPhi == true
            ? $"Yêu cầu sửa chữa \"{yeuCau.NoiDung}\" đã hoàn tất. Miễn phí."
            : $"Yêu cầu sửa chữa \"{yeuCau.NoiDung}\" đã hoàn tất. Chi phí quyết toán: {yeuCau.ChiPhiThucTe?.ToString("N0")} VNĐ.";

        var loaiThongBao = LoaiThongBao.YeuCauSuaChua;
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

    private async Task TryCreateRepairInvoiceAsync(Domain.Entities.YeuCauSuaChua yeuCau, CancellationToken cancellationToken)
    {
        // Bỏ qua nếu miễn phí hoặc không có chi phí thực tế
        if (yeuCau.IsMienPhi == true || yeuCau.ChiPhiThucTe is null or <= 0)
        {
            _logger.LogInformation("YeuCauSuaChua {Id}: Miễn phí hoặc không có chi phí — bỏ qua tạo hóa đơn.", yeuCau.Id);
            return;
        }

        var canHo = await _canHoRepository.GetByIdAsync(yeuCau.CanHoId, cancellationToken);
        if (canHo == null)
        {
            _logger.LogWarning("YeuCauSuaChua {Id}: Không tìm thấy căn hộ {CanHoId} — bỏ qua tạo hóa đơn.", yeuCau.Id, yeuCau.CanHoId);
            return;
        }

        // HD-SC-{MaCanHo}-{YeuCauId}: duy nhất và có thể trace ngược
        string maHoaDon = $"HD-SC-{canHo.MaCanHo}-{yeuCau.Id}";
        var ngayHan = _dateTimeProvider.Now.AddDays(7);

        var hoaDonResult = _billingDomainService.CreateInvoiceForRepair(yeuCau, canHo, maHoaDon, ngayHan);
        if (hoaDonResult.IsFailure)
        {
            _logger.LogWarning("YeuCauSuaChua {Id}: Tạo hóa đơn thất bại — {Error}.", yeuCau.Id, hoaDonResult.Errors.FirstOrDefault()?.Description);
            return;
        }

        await _hoaDonRepository.AddAsync(hoaDonResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Ghi nhận HoaDonId ngược vào yêu cầu để có thể tra cứu
        yeuCau.MarkAsBilled(hoaDonResult.Value.Id);
        _yeuCauCommandRepository.Update(yeuCau);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("YeuCauSuaChua {Id}: Đã tạo hóa đơn {MaHoaDon} thành công.", yeuCau.Id, maHoaDon);
    }
}
