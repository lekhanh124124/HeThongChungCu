using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetYeuCauPhuongTienById;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using MediatR;
using System.Text.Json;

namespace HeThongChungCu.Application.Features.QLPhuongTien.EventHandlers;

public class YeuCauPhuongTienCreatedEventHandler : INotificationHandler<YeuCauPhuongTienCreatedEvent>
{
    private readonly ITaiKhoanCommandRepository _taiKhoanRepository;
    private readonly IThongBaoCommandRepository _thongBaoRepository;
    private readonly IYeuCauPhuongTienQueryRepository _yeuCauQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public YeuCauPhuongTienCreatedEventHandler(
        ITaiKhoanCommandRepository taiKhoanRepository,
        IThongBaoCommandRepository thongBaoRepository,
        IYeuCauPhuongTienQueryRepository yeuCauQueryRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _taiKhoanRepository = taiKhoanRepository;
        _thongBaoRepository = thongBaoRepository;
        _yeuCauQueryRepository = yeuCauQueryRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task Handle(YeuCauPhuongTienCreatedEvent notification, CancellationToken cancellationToken)
    {
        var yeuCau = notification.YeuCau;
        var loaiThongBao = LoaiThongBao.YeuCauPhuongTien;
        string title = "Yêu cầu phương tiện mới";
        string content = $"Có một yêu cầu phương tiện mới cho căn hộ ID: {yeuCau.CanHoId}. Loại: {yeuCau.LoaiHanhDongYeuCauId.Name}";

        // 1. Tìm người nhận thuộc BQL (Manager, Staff & Admin)
        var managerIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Manager, cancellationToken);
        var staffIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Staff, cancellationToken);
        // var adminIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Admin, cancellationToken);

        var allRecipientIds = managerIds.Concat(staffIds).Distinct().ToList();

        if (allRecipientIds.Count == 0) return;

        // Lấy dữ liệu đầy đủ để đưa vào Metadata
        var spec = new GetYeuCauPhuongTienByIdSpecification(yeuCau.Id);
        var listResponse = await _yeuCauQueryRepository.GetListResponseByIdAsync(spec, cancellationToken);
        string? metadataJson = listResponse != null ? JsonSerializer.Serialize(listResponse) : null;

        // 2. Tạo thực thể ThongBao và Phân bổ
        var thongBao = new HeThongChungCu.Domain.Entities.ThongBao(title, content, loaiThongBao, yeuCau.Id.ToString(), metadataJson);
        foreach (var recipientId in allRecipientIds)
        {
            thongBao.ThemPhanBo(recipientId);
        }

        // 3. Lưu vào Database
        await _thongBaoRepository.AddAsync(thongBao, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Đẩy thông báo thời gian thực (Push)
        await _notificationService.PushToUsersAsync(allRecipientIds, new
        {
            Id = thongBao.Id,
            TieuDe = title,
            NoiDung = content,
            LoaiThongBaoId = loaiThongBao.Value,
            TenLoaiThongBao = loaiThongBao.Name,
            ReferenceId = yeuCau.Id.ToString(),
            Metadata = listResponse,
            CreatedAt = DateTimeOffset.Now
        }, cancellationToken);
    }
}
