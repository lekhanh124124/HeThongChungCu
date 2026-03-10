using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.CanHo.Commands.CreateCanHo;

public class CreateCanHoCommandHandler : ICommandHandler<CreateCanHoCommand, CanHoDetailResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IToaNhaEFRepository _toaNhaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCanHoCommandHandler(
        ICanHoEFRepository canHoRepository,
        IToaNhaEFRepository toaNhaRepository,
        IUnitOfWork unitOfWork)
    {
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CanHoDetailResponse>> Handle(CreateCanHoCommand request, CancellationToken cancellationToken)
    {
        var toaNhaExists = await _toaNhaRepository.AnyAsync(request.ToaNhaId, cancellationToken);
        if (!toaNhaExists)
            return Result.Failure<CanHoDetailResponse>(ToaNhaErrors.NotFound);

        var maExists = await _canHoRepository.MaCanHoExistsAsync(request.MaCanHo, cancellationToken);
        if (maExists)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.MaCanHoAlreadyExists);

        var loaiCanHo = LoaiCanHo.FromValue(request.LoaiCanHoId);
        var tinhTrangCanHo = TinhTrangCanHo.Trong;

        var canHo = new HeThongChungCu.Domain.Entities.ChungCu.CanHo(
            request.ToaNhaId,
            request.MaCanHo,
            request.DienTich,
            request.Tang,
            request.SoPhongNgu,
            request.SoPhongTam,
            loaiCanHo!.Value,
            tinhTrangCanHo.Value);

        await _canHoRepository.AddAsync(canHo, cancellationToken);

        return Result.Success(new CanHoDetailResponse
        {
            Id = canHo.Id,
            ToaNhaId = canHo.ToaNhaId,
            MaCanHo = canHo.MaCanHo,
            DienTich = canHo.DienTich,
            Tang = canHo.Tang,
            SoPhongNgu = canHo.SoPhongNgu,
            SoPhongTam = canHo.SoPhongTam,
            LoaiCanHoId = canHo.LoaiCanHoId,
            TenLoaiCanHo = LoaiCanHo.FromValue(canHo.LoaiCanHoId)?.Name ?? string.Empty,
            TinhTrangCanHoId = canHo.TinhTrangCanHoId,
            TenTinhTrangCanHo = TinhTrangCanHo.FromValue(canHo.TinhTrangCanHoId)?.Name ?? string.Empty
        });
    }
}
