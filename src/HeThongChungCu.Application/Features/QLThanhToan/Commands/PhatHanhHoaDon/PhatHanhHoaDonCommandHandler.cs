using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.PhatHanhHoaDon;

public class PhatHanhHoaDonCommandHandler : ICommandHandler<PhatHanhHoaDonCommand, bool>
{
    private readonly IDotThanhToanCommandRepository _dotRepository;
    private readonly IHoaDonCommandRepository _hoaDonRepository;
    private readonly IBillingDomainService _billingService;
    private readonly IUnitOfWork _unitOfWork;

    public PhatHanhHoaDonCommandHandler(
        IDotThanhToanCommandRepository dotRepository,
        IHoaDonCommandRepository hoaDonRepository,
        IBillingDomainService billingService,
        IUnitOfWork unitOfWork)
    {
        _dotRepository = dotRepository;
        _hoaDonRepository = hoaDonRepository;
        _billingService = billingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(PhatHanhHoaDonCommand request, CancellationToken cancellationToken)
    {
        var dot = await _dotRepository.GetByIdAsync(request.DotThanhToanId, cancellationToken);
        if (dot == null)
            return Result.Failure<bool>(new Error("DotThanhToan.NotFound", "Không tìm thấy đợt thanh toán."));

        List<HoaDon> hoaDons;
        if (request.HoaDonIds != null && request.HoaDonIds.Count > 0)
        {
            // Trường hợp phát hành theo danh sách ID cụ thể
            hoaDons = await _hoaDonRepository.GetByIdsAsync(request.HoaDonIds, cancellationToken);
        }
        else
        {
            // Trường hợp phát hành toàn bộ hóa đơn dự thảo trong đợt
            var allHoaDons = await _hoaDonRepository.GetByDotThanhToanAsync(request.DotThanhToanId, cancellationToken);
            hoaDons = allHoaDons.Where(x => x.TrangThaiHoaDonId == TrangThaiHoaDon.ChoDuyet).ToList();
        }

        if (hoaDons.Count == 0)
            return Result.Failure<bool>(new Error("HoaDon.NotFound", "Không tìm thấy danh sách hóa đơn hợp lệ cần phát hành."));

        // Sử dụng Domain Service để thực hiện phát hành theo lô
        var result = _billingService.PhatHanhBatch(dot, hoaDons);
        if (result.IsFailure)
            return Result.Failure<bool>(result.Errors);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
