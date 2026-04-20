using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.NhapBaoGiaYeuCauSuaChua;

public class NhapBaoGiaYeuCauSuaChuaCommandHandler : ICommandHandler<NhapBaoGiaYeuCauSuaChuaCommand, YeuCauSuaChuaDetailResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly IYeuCauSuaChuaQueryRepository _queryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NhapBaoGiaYeuCauSuaChuaCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        IYeuCauSuaChuaQueryRepository queryRepository,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _queryRepository = queryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaDetailResponse>> Handle(NhapBaoGiaYeuCauSuaChuaCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Request
        var ycsc = await _ycscRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ycsc == null)
            return YeuCauSuaChuaErrors.NotFoundById(request.Id);

        // 2. Logic
        ycsc.NhapBaoGia(request.ChiPhiDuKien, request.IsMienPhi, request.GhiChuBaoGia);

        // 3. Persistence
        _ycscRepository.Update(ycsc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Build Response using Query Repository
        var result = await _queryRepository.GetByIdAsync(new GetYeuCauSuaChuaByIdSpecification(ycsc.Id), cancellationToken);

        return result != null
            ? Result.Success(result)
            : Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.NotFoundById(ycsc.Id));
    }
}
