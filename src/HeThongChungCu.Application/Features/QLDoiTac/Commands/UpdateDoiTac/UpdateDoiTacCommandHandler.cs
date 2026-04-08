using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetDoiTacById;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.UpdateDoiTac;

public class UpdateDoiTacCommandHandler : ICommandHandler<UpdateDoiTacCommand, DoiTacDetailResponse>
{
    private readonly IDoiTacCommandRepository _doiTacCommandRepository;
    private readonly IDoiTacQueryRepository _doiTacQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDoiTacCommandHandler(
        IDoiTacCommandRepository doiTacCommandRepository,
        IDoiTacQueryRepository doiTacQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _doiTacCommandRepository = doiTacCommandRepository;
        _doiTacQueryRepository = doiTacQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DoiTacDetailResponse>> Handle(UpdateDoiTacCommand request, CancellationToken cancellationToken)
    {
        var doiTac = await _doiTacCommandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (doiTac == null)
            return Result.Failure<DoiTacDetailResponse>(DoiTacErrors.NotFoundById(request.Id));

        doiTac.UpdateInfo(
            request.TenDoiTac,
            request.TenCongTy,
            request.NguoiDaiDien,
            request.SoGiayPhepKD,
            request.MaSoThue,
            request.DiaChi,
            request.SoDienThoai,
            request.Email,
            request.GhiChu);

        _doiTacCommandRepository.Update(doiTac);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var spec = new GetDoiTacByIdSpecification(doiTac.Id);
        var result = await _doiTacQueryRepository.GetByIdAsync(spec, cancellationToken);

        if (result == null)
            return Result.Failure<DoiTacDetailResponse>(DoiTacErrors.NotFound);

        return Result.Success(result);
    }
}
