using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeDichVu;

public class RevokeDichVuCommandHandler : ICommandHandler<RevokeDichVuCommand, bool>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeDichVuCommandHandler(IDichVuCommandRepository dichVuCommandRepository, IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RevokeDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVus = await _dichVuCommandRepository.GetByIdsAsync(request.Ids, cancellationToken);
        var foundIds = dichVus.Select(x => x.Id).ToList();
        var missingIds = request.Ids.Except(foundIds).ToList();

        if (missingIds.Count != 0)
        {
            return DichVuErrors.NotFoundByIds(missingIds);
        }

        foreach (var dichVu in dichVus)
        {
            dichVu.Revoke();
            _dichVuCommandRepository.Update(dichVu);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
