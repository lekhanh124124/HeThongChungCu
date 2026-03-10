using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;

public class UpdateToaNhaCommandHandler : ICommandHandler<UpdateToaNhaCommand, ToaNhaDetailResponse>
{
    private readonly IToaNhaEFRepository _toaNhaRepository;

    public UpdateToaNhaCommandHandler(
        IToaNhaEFRepository toaNhaRepository)
    {
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<ToaNhaDetailResponse>> Handle(UpdateToaNhaCommand request, CancellationToken cancellationToken)
    {
        var toaNha = await _toaNhaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (toaNha is null)
            return Result.Failure<ToaNhaDetailResponse>(ToaNhaErrors.NotFoundById(request.Id));

        var trangThaiToaNha = TrangThaiToaNha.FromValue(request.TrangThaiToaNhaId);

        toaNha.Update(
            request.TenToaNha,
            request.SoTang,
            request.SoTangHam,
            request.DiaChi,
            request.MoTa,
            trangThaiToaNha!.Value);

        _toaNhaRepository.Update(toaNha);

        // TransactionBehavior will handle the commit
        return Result.Success(new ToaNhaDetailResponse
        {
            Id = toaNha.Id,
            MaToaNha = toaNha.MaToaNha,
            TenToaNha = toaNha.TenToaNha,
            SoTang = toaNha.SoTang,
            SoTangHam = toaNha.SoTangHam,
            DiaChi = toaNha.DiaChi,
            MoTa = toaNha.MoTa,
            TrangThaiToaNhaId = trangThaiToaNha!.Value,
            TenTrangThaiToaNha = trangThaiToaNha!.Name
        });
    }
}
