using System.Text.Json;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.QuetPhanAnhQuaHan;

public class QuetPhanAnhQuaHanCommandHandler : IRequestHandler<QuetPhanAnhQuaHanCommand, Result>
{
    private readonly IYeuCauPhanAnhCommandRepository _phanAnhRepository;
    private readonly ITaiKhoanCommandRepository _taiKhoanRepository;
    private readonly IThongBaoCommandRepository _thongBaoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly INotificationService _notificationService;
    private readonly ILogger<QuetPhanAnhQuaHanCommandHandler> _logger;

    public QuetPhanAnhQuaHanCommandHandler(
        IYeuCauPhanAnhCommandRepository phanAnhRepository,
        ITaiKhoanCommandRepository taiKhoanRepository,
        IThongBaoCommandRepository thongBaoRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        INotificationService notificationService,
        ILogger<QuetPhanAnhQuaHanCommandHandler> logger)
    {
        _phanAnhRepository = phanAnhRepository;
        _taiKhoanRepository = taiKhoanRepository;
        _thongBaoRepository = thongBaoRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result> Handle(QuetPhanAnhQuaHanCommand request, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.Now;
        var overduePhanAnhs = await _phanAnhRepository.GetOverdueNotNotifiedAsync(now, cancellationToken);

        if (overduePhanAnhs.Count == 0)
        {
            return Result.Success();
        }

        _logger.LogInformation("Found {Count} overdue Phan Anh to notify.", overduePhanAnhs.Count);

        var managerIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Manager, cancellationToken);
        var staffIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Staff, cancellationToken);
        var adminIds = await _taiKhoanRepository.GetNguoiDungIdsByRoleAsync(Role.Admin, cancellationToken);
        var allRecipientIds = managerIds.Concat(staffIds).Concat(adminIds).Distinct().ToList();

        if (allRecipientIds.Count == 0)
        {
            _logger.LogWarning("No BQL members found to receive overdue notifications.");
            return Result.Success();
        }

        foreach (var phanAnh in overduePhanAnhs)
        {
            string title = "CẢNH BÁO QUÁ HẠN: Yêu cầu phản ánh";
            string content = $"Phản ánh #{phanAnh.Id} (Căn hộ {phanAnh.CanHoId}) đã quá hạn xử lý vào lúc {phanAnh.HanPhanHoi:dd/MM/yyyy HH:mm}. Vui lòng kiểm tra và xử lý ngay!";

            var thongBao = new HeThongChungCu.Domain.Entities.ThongBao(title, content, LoaiThongBao.YeuCauPhanAnh, phanAnh.Id.ToString(), null);
            foreach (var recipientId in allRecipientIds)
            {
                thongBao.ThemPhanBo(recipientId);
            }

            await _thongBaoRepository.AddAsync(thongBao, cancellationToken);
            phanAnh.MarkAsOverdueNotified();

            // Fire real-time notification
            await _notificationService.PushToUsersAsync(allRecipientIds, new
            {
                Id = thongBao.Id,
                TieuDe = title,
                NoiDung = content,
                LoaiThongBaoId = LoaiThongBao.YeuCauPhanAnh.Value,
                TenLoaiThongBao = LoaiThongBao.YeuCauPhanAnh.Name,
                ReferenceId = phanAnh.Id.ToString(),
                CreatedAt = now
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
