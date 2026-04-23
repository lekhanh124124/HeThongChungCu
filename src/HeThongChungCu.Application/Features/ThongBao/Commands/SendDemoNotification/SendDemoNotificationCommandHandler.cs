using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.ThongBao.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ThongBao.Commands.SendDemoNotification;

public class SendDemoNotificationCommandHandler : ICommandHandler<SendDemoNotificationCommand, ThongBaoResponse>
{
    private readonly IThongBaoCommandRepository _thongBaoCommandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SendDemoNotificationCommandHandler(
        IThongBaoCommandRepository thongBaoCommandRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _thongBaoCommandRepository = thongBaoCommandRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<ThongBaoResponse>> Handle(SendDemoNotificationCommand request, CancellationToken cancellationToken)
    {
        // Nếu UserId truyền vào là null thì lấy UserId của người dùng hiện tại
        var targetUserId = request.UserId ?? _currentUserService.UserId;
        if (targetUserId == null)
            return UserErrors.NotFound;

        // Tạo entity ThongBao với các tham số hard code
        var thongBao = new Domain.Entities.ThongBao(
            "Thông báo Demo",
            "Đây là thông báo demo được tạo từ thực thể (entity) và đã được lưu vào hệ thống.",
            LoaiThongBao.HeThong,
            null,
            null
        );

        // Phân bổ thông báo cho người dùng đích
        thongBao.ThemPhanBo(targetUserId.Value);

        // Lưu vào cơ sở dữ liệu
        await _thongBaoCommandRepository.AddAsync(thongBao, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Lấy thông tin phân bổ vừa tạo (vì chỉ có 1 phân bổ nên lấy cái đầu tiên)
        var phanBo = thongBao.PhanBoThongBaos.First();

        // Map sang ThongBaoResponse để trả về và đẩy qua SignalR
        var response = new ThongBaoResponse
        {
            Id = phanBo.Id,
            ThongBaoId = thongBao.Id,
            TieuDe = thongBao.TieuDe,
            NoiDung = thongBao.NoiDung,
            LoaiThongBaoId = thongBao.LoaiThongBao.Value,
            TenLoaiThongBao = thongBao.LoaiThongBao.Name,
            ReferenceId = thongBao.ReferenceId,
            Metadata = thongBao.Metadata,
            IsRead = phanBo.IsRead,
            CreatedAt = thongBao.CreatedAt,
            ReadAt = phanBo.ReadAt
        };

        // Đẩy thông báo thời gian thực qua SignalR
        await _notificationService.PushToUsersAsync(new[] { targetUserId.Value }, response, cancellationToken);

        return response;
    }
}
