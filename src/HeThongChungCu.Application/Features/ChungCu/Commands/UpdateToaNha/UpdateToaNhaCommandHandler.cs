using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.ChungCu.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.UpdateToaNha;

public class UpdateToaNhaCommandHandler : ICommandHandler<UpdateToaNhaCommand, ToaNhaResponse>
{
    private readonly IToaNhaEFRepository _toaNhaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateToaNhaCommandHandler(IToaNhaEFRepository toaNhaRepository, IUnitOfWork unitOfWork)
    {
        _toaNhaRepository = toaNhaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ToaNhaResponse>> Handle(UpdateToaNhaCommand request, CancellationToken cancellationToken)
    {
        var toaNha = await _toaNhaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (toaNha is null)
            return Result.Failure<ToaNhaResponse>(ToaNhaErrors.NotFoundById(request.Id));

        toaNha.Update(request.TenToaNha, request.SoTang, request.SoTangHam, request.DiaChi, request.MoTa, request.TrangThaiToaNhaId);
        _toaNhaRepository.Update(toaNha);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ToaNhaResponse
        {
            Id = toaNha.Id,
            MaToaNha = toaNha.MaToaNha,
            TenToaNha = toaNha.TenToaNha,
            SoTang = toaNha.SoTang
        });
    }
}
