using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.XoaYeuCauPhuongTien;

public class XoaYeuCauPhuongTienCommandHandler : ICommandHandler<XoaYeuCauPhuongTienCommand, bool>
{
    private readonly IYeuCauPhuongTienEFRepository _yeuCauRepository;
    private readonly IUnitOfWork _unitOfWork;

    public XoaYeuCauPhuongTienCommandHandler(IYeuCauPhuongTienEFRepository yeuCauRepository, IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(XoaYeuCauPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var yeuCaus = (await _yeuCauRepository.GetByIdsAsync(request.Ids, cancellationToken)).ToList();

        if (yeuCaus.Count != request.Ids.Count)
        {
            var foundIds = yeuCaus.Select(y => y.Id).ToList();
            var missingIds = request.Ids.Except(foundIds).ToList();
            return Result.Failure<bool>(YeuCauPhuongTienErrors.NotFoundByIds(missingIds));
        }

        _yeuCauRepository.DeleteRange(yeuCaus);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
