using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;

public class UpdateToaNhaCommandHandler : ICommandHandler<UpdateToaNhaCommand, ToaNhaDetailResponse>
{
    private readonly IToaNhaCommandRepository _toaNhaRepository;

    public UpdateToaNhaCommandHandler(
        IToaNhaCommandRepository toaNhaRepository)
    {
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<ToaNhaDetailResponse>> Handle(UpdateToaNhaCommand request, CancellationToken cancellationToken)
    {
        var toaNha = await _toaNhaRepository.GetToaNhaByIdAsync(request.Id, cancellationToken);
        if (toaNha is null)
            return Result.Failure<ToaNhaDetailResponse>(ToaNhaErrors.NotFoundById(request.Id));

        var trangThaiToaNha = TrangThaiToaNha.FromValue(request.TrangThaiToaNhaId);

        // Nếu mã thay đổi, kiểm tra trùng mã
        if (request.MaToaNha != toaNha.MaToaNha)
        {
            var maExists = toaNha.Tangs.Any(t => t.MaTang == request.MaToaNha);
            if (maExists)
                return Result.Failure<ToaNhaDetailResponse>(ToaNhaErrors.MaToaNhaAlreadyExists);
        }
        toaNha.Update(
            request.MaToaNha,
            request.TenToaNha,
            request.DiaChi,
            request.MoTa,
            trangThaiToaNha);

        _toaNhaRepository.Update(toaNha);

        // TransactionBehavior will handle the commit
        return Result.Success(new ToaNhaDetailResponse
        {
            Id = toaNha.Id,
            MaToaNha = toaNha.MaToaNha,
            TenToaNha = toaNha.TenToaNha,
            DiaChi = toaNha.DiaChi.FullAddress,
            MoTa = toaNha.MoTa,
            TrangThaiToaNhaId = toaNha.TrangThaiToaNhaId.Value,
            TenTrangThaiToaNha = toaNha.TrangThaiToaNhaId.Name
        });
    }
}
