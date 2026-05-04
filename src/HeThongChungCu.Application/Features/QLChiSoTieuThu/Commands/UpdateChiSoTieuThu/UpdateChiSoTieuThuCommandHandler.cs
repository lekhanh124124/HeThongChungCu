using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetChiSoById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.UpdateChiSoTieuThu;

public class UpdateChiSoTieuThuCommandHandler : ICommandHandler<UpdateChiSoTieuThuCommand, ChiSoDetailResponse>
{
    private readonly IChiSoTieuThuCommandRepository _chiSoCommandRepository;
    private readonly IChiSoTieuThuQueryRepository _chiSoQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateChiSoTieuThuCommandHandler(
        IChiSoTieuThuCommandRepository chiSoCommandRepository,
        IChiSoTieuThuQueryRepository chiSoQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _chiSoCommandRepository = chiSoCommandRepository;
        _chiSoQueryRepository = chiSoQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChiSoDetailResponse>> Handle(UpdateChiSoTieuThuCommand request, CancellationToken cancellationToken)
    {
        var chiSo = await _chiSoCommandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (chiSo == null)
            return ChiSoTieuThuErrors.NotFoundById(request.Id);

        chiSo.Update(
            request.ChiSoCu,
            request.ChiSoMoi,
            request.Thang,
            request.Nam,
            request.NgayGhiNhan,
            request.AnhDongHoId,
            request.GhiChu);

        _chiSoCommandRepository.Update(chiSo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _chiSoQueryRepository.GetByIdAsync(new GetChiSoByIdSpecification(request.Id), cancellationToken);
        return response != null
            ? response
            : ChiSoTieuThuErrors.NotFound;
    }
}
