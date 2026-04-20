using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.CreateToaNha;

public class CreateToaNhaCommandHandler : ICommandHandler<CreateToaNhaCommand, ToaNhaDetailResponse>
{
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    public CreateToaNhaCommandHandler(
        IToaNhaCommandRepository toaNhaRepository)
    {
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<ToaNhaDetailResponse>> Handle(CreateToaNhaCommand request, CancellationToken cancellationToken)
    {
        var exists = await _toaNhaRepository.MaToaNhaExistsAsync(request.MaToaNha, cancellationToken);
        if (exists)
            return ToaNhaErrors.MaToaNhaAlreadyExists;

        var trangThaiToaNha = TrangThaiToaNha.DangHoatDong;
        var toaNha = new Domain.Entities.ToaNha(
            request.MaToaNha,
            request.TenToaNha,
            request.Block,
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
            Block = toaNha.Block,
            DiaChi = toaNha.DiaChi.FullAddress,
            MoTa = toaNha.MoTa,
            TrangThaiToaNhaId = toaNha.TrangThaiToaNhaId.Value,
            TenTrangThaiToaNha = trangThaiToaNha.Name
        });
    }
}
