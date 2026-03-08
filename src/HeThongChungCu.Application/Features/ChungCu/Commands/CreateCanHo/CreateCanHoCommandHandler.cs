using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.ChungCu.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.CreateCanHo;

public class CreateCanHoCommandHandler : ICommandHandler<CreateCanHoCommand, CanHoResponse>
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

    public async Task<Result<CanHoResponse>> Handle(CreateCanHoCommand request, CancellationToken cancellationToken)
    {
        var toaNhaExists = await _toaNhaRepository.AnyAsync(request.ToaNhaId, cancellationToken);
        if (!toaNhaExists)
            return Result.Failure<CanHoResponse>(ToaNhaErrors.NotFound);

        var maExists = await _canHoRepository.MaCanHoExistsAsync(request.MaCanHo, cancellationToken);
        if (maExists)
            return Result.Failure<CanHoResponse>(CanHoErrors.MaCanHoAlreadyExists);

        var canHo = new CanHo(
            request.ToaNhaId,
            request.MaCanHo,
            request.DienTich,
            request.Tang,
            request.SoPhongNgu,
            request.SoPhongTam,
            request.TinhTrangCanHoId);

        await _canHoRepository.AddAsync(canHo, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CanHoResponse
        {
            Id = canHo.Id,
            ToaNhaId = canHo.ToaNhaId,
            MaCanHo = canHo.MaCanHo,
            DienTich = canHo.DienTich,
            Tang = canHo.Tang,
            SoPhongNgu = canHo.SoPhongNgu,
            SoPhongTam = canHo.SoPhongTam,
            TinhTrangCanHoId = canHo.TinhTrangCanHoId
        });
    }
}
