using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.XoaYeuCauCuTru;

public class XoaYeuCauCuTruCommandHandler : ICommandHandler<XoaYeuCauCuTruCommand, bool>
{
    private readonly IYeuCauCuTruCommandRepository _yeuCauRepository;
    private readonly IUnitOfWork _unitOfWork;

    public XoaYeuCauCuTruCommandHandler(IYeuCauCuTruCommandRepository yeuCauRepository, IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(XoaYeuCauCuTruCommand request, CancellationToken cancellationToken)
    {
        var yeuCaus = (await _yeuCauRepository.GetByIdsAsync(request.Ids, cancellationToken)).ToList();

        if (yeuCaus.Count != request.Ids.Count)
        {
            var foundIds = yeuCaus.Select(y => y.Id).ToList();
            var missingIds = request.Ids.Except(foundIds).ToList();
            return YeuCauCuTruErrors.NotFoundByIds(missingIds);
        }

        _yeuCauRepository.DeleteRange(yeuCaus);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
