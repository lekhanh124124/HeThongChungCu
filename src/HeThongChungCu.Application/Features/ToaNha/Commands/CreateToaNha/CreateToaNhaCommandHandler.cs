using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.CreateToaNha;

public class CreateToaNhaCommandHandler : ICommandHandler<CreateToaNhaCommand, ToaNhaDetailResponse>
{
    private readonly IToaNhaEFRepository _toaNhaRepository;
    public CreateToaNhaCommandHandler(
        IToaNhaEFRepository toaNhaRepository)
    {
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<ToaNhaDetailResponse>> Handle(CreateToaNhaCommand request, CancellationToken cancellationToken)
    {
        var exists = await _toaNhaRepository.MaToaNhaExistsAsync(request.MaToaNha, cancellationToken);
        if (exists)
            return Result.Failure<ToaNhaDetailResponse>(ToaNhaErrors.MaToaNhaAlreadyExists);

        var trangThaiToaNha = TrangThaiToaNha.DangHoatDong;
        var toaNha = new HeThongChungCu.Domain.Entities.ChungCu.ToaNha(
            request.MaToaNha,
            request.TenToaNha,
            request.DiaChi,
            request.MoTa,
            trangThaiToaNha);

        await _toaNhaRepository.AddAsync(toaNha, cancellationToken);

        // TransactionBehavior will handle the commit

        return Result.Success(new ToaNhaDetailResponse
        {
            Id = toaNha.Id,
            MaToaNha = toaNha.MaToaNha,
            TenToaNha = toaNha.TenToaNha,
            DiaChi = toaNha.DiaChi,
            MoTa = toaNha.MoTa,
            TrangThaiToaNhaId = toaNha.TrangThaiToaNhaId.Value,
            TenTrangThaiToaNha = trangThaiToaNha.Name
        });
    }
}
