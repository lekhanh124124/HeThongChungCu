using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.RevokeHopDong;

public class RevokeHopDongCommandHandler : ICommandHandler<RevokeHopDongCommand, bool>
{
    private readonly IDoiTacCommandRepository _doiTacCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeHopDongCommandHandler(IDoiTacCommandRepository doiTacCommandRepository, IUnitOfWork unitOfWork)
    {
        _doiTacCommandRepository = doiTacCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RevokeHopDongCommand request, CancellationToken cancellationToken)
    {
        var doiTac = await _doiTacCommandRepository.GetByIdWithHopDongsAsync(request.DoiTacId, cancellationToken);
        if (doiTac == null)
            return Result.Failure<bool>(DoiTacErrors.NotFoundById(request.DoiTacId));

        foreach (var id in request.Ids)
        {
            var contract = doiTac.HopDongs.FirstOrDefault(h => h.Id == id);
            if (contract != null)
            {
                contract.Revoke();
            }
        }

        _doiTacCommandRepository.Update(doiTac);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
