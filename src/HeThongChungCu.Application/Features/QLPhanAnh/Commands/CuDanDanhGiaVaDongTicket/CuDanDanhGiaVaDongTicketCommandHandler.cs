using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.CuDanDanhGiaVaDongTicket;

public class CuDanDanhGiaVaDongTicketCommandHandler : ICommandHandler<CuDanDanhGiaVaDongTicketCommand, PhanAnhResponse>
{
    private readonly IYeuCauPhanAnhCommandRepository _phanAnhCommandRepository;
    private readonly IYeuCauPhanAnhQueryRepository _phanAnhQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CuDanDanhGiaVaDongTicketCommandHandler(
        IYeuCauPhanAnhCommandRepository phanAnhCommandRepository,
        IYeuCauPhanAnhQueryRepository phanAnhQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _phanAnhCommandRepository = phanAnhCommandRepository;
        _phanAnhQueryRepository = phanAnhQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhanAnhResponse>> Handle(CuDanDanhGiaVaDongTicketCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch Feedback
        var phanAnh = await _phanAnhCommandRepository.GetByIdAsync(command.PhanAnhId, cancellationToken);
        if (phanAnh == null)
            return PhanAnhErrors.NotFound;

        // 2. Domain logic (CuDanDanhGiaVaDongTicket)
        var result = phanAnh.CuDanDanhGiaVaDongTicket(command.DiemDanhGia, command.NhanXetDanhGia);
        if (result.IsFailure)
            return result.Errors;

        // 3. Save
        _phanAnhCommandRepository.Update(phanAnh);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Query detailed response
        var response = await _phanAnhQueryRepository.GetByIdAsync(new GetPhanAnhByIdSpecification(phanAnh.Id), cancellationToken);

        return response != null
            ? Result.Success<PhanAnhResponse>(response)
            : Result.Failure<PhanAnhResponse>(PhanAnhErrors.NotFound);
    }
}
