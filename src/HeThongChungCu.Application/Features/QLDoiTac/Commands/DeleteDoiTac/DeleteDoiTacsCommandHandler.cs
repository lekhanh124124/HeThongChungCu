using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.DeleteDoiTac;

public class DeleteDoiTacsCommandHandler : ICommandHandler<DeleteDoiTacsCommand, IReadOnlyList<DoiTacResponse>>
{
    private readonly IDoiTacCommandRepository _doiTacCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDoiTacsCommandHandler(
        IDoiTacCommandRepository doiTacCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _doiTacCommandRepository = doiTacCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<DoiTacResponse>>> Handle(DeleteDoiTacsCommand request, CancellationToken cancellationToken)
    {
        var doiTacs = await _doiTacCommandRepository.GetByIdsAsync(request.Ids, cancellationToken);
        
        if (doiTacs.Count != request.Ids.Count)
        {
            var foundIds = doiTacs.Select(x => x.Id).ToList();
            var missingIds = request.Ids.Except(foundIds).ToList();
            return new Error("DoiTac.SomeNotFound", $"Không tìm thấy một số đơn vị cung cấp: {string.Join(", ", missingIds)}");
        }

        var doiTacsWithHopDongs = doiTacs.Where(d => d.HopDongs.Any()).ToList();
        if (doiTacsWithHopDongs.Any())
        {
            var invalidIds = doiTacsWithHopDongs.Select(d => d.Id).ToList();
            return new Error("DoiTac.HasHopDongs", $"Không thể xóa đối tác đã có hợp đồng: {string.Join(", ", invalidIds)}");
        }

        var response = doiTacs.Select(t => new DoiTacResponse
        {
            Id = t.Id,
            TenDoiTac = t.TenDoiTac,
            TenCongTy = t.TenCongTy,
            NguoiDaiDien = t.NguoiDaiDien,
            SoDienThoai = t.SoDienThoai?.Value,
            Email = t.Email?.Value
        }).ToList();

        _doiTacCommandRepository.RemoveRange(doiTacs);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<IReadOnlyList<DoiTacResponse>>(response);
    }
}
