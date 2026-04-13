using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using MediatR;
using System.Text.Json;

namespace HeThongChungCu.Application.Features.QLCuTru.EventHandlers;

public class YeuCauCuTruCreatedEventHandler : INotificationHandler<YeuCauCuTruCreatedEvent>
{
    private readonly ITaiKhoanCommandRepository _taiKhoanRepository;
    private readonly IThongBaoCommandRepository _thongBaoRepository;
    private readonly IYeuCauCuTruQueryRepository _yeuCauQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public YeuCauCuTruCreatedEventHandler(
        ITaiKhoanCommandRepository taiKhoanRepository,
        IThongBaoCommandRepository thongBaoRepository,
        IYeuCauCuTruQueryRepository yeuCauQueryRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _taiKhoanRepository = taiKhoanRepository;
        _thongBaoRepository = thongBaoRepository;
        _yeuCauQueryRepository = yeuCauQueryRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task Handle(YeuCauCuTruCreatedEvent notification, CancellationToken cancellationToken)
    {
        var yeuCau = notification.YeuCau;
        var loaiThongBao = LoaiThongBao.YeuCauCuTru;

        string title = "Yêu cầu cư trú mới";
        string content = $"Có một yêu cầu cư trú mới cho căn hộ ID: {yeuCau.CanHoId}.";

        // 1. Tìm người nhận thuộc BQL (Manager, Staff & Admin)
        var managerIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Manager, cancellationToken);
        var staffIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Staff, cancellationToken);
        var adminIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Admin, cancellationToken);

        var allRecipientIds = managerIds.Concat(staffIds).Concat(adminIds).Distinct().ToList();

        if (allRecipientIds.Count == 0) return;

        // Lấy dữ liệu đầy đủ để đưa vào Metadata (Giúp Frontend hiển thị ngay mà không cần gọi API)
        var listResponse = await _yeuCauQueryRepository.GetListResponseByIdAsync(new GetYeuCauCuTruByIdSpecification(yeuCau.Id), cancellationToken);
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

        // 4. Đẩy thông báo thời gian thực (Push) - Truyền Metadata dưới dạng Object để Frontend dễ sử dụng
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
