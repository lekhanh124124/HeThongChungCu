using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.ChungCu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.DeleteCanHo;

public class DeleteCanHoCommandHandler : ICommandHandler<DeleteCanHoCommand, IReadOnlyList<CanHoResponse>>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCanHoCommandHandler(ICanHoEFRepository canHoRepository, IUnitOfWork unitOfWork)
    {
        _canHoRepository = canHoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<CanHoResponse>>> Handle(DeleteCanHoCommand request, CancellationToken cancellationToken)
    {
        var canHos = await _canHoRepository.GetByIdsAsync(request.Ids, cancellationToken);

        var notFoundIds = request.Ids.Except(canHos.Select(c => c.Id)).ToList();
        if (notFoundIds.Count > 0)
        {
            var ids = string.Join(", ", notFoundIds);
            return Result.Failure<IReadOnlyList<CanHoResponse>>(new Error(
                "CanHo.NotFound",
                $"Không tìm thấy căn hộ với ID: {ids}."));
        }

        var response = canHos.Select(c => new CanHoResponse
        {
            Id = c.Id,
            ToaNhaId = c.ToaNhaId,
            MaCanHo = c.MaCanHo,
            DienTich = c.DienTich,
            Tang = c.Tang,
            SoPhongNgu = c.SoPhongNgu,
            SoPhongTam = c.SoPhongTam,
            TinhTrangCanHoId = c.TinhTrangCanHoId
        }).ToList();

        foreach (var canHo in canHos)
        {
            _canHoRepository.Remove(canHo);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<IReadOnlyList<CanHoResponse>>(response);
    }
}
