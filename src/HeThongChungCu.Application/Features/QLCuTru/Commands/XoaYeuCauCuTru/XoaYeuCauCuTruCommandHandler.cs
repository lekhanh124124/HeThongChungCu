using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.XoaYeuCauCuTru;

public class XoaYeuCauCuTruCommandHandler : ICommandHandler<XoaYeuCauCuTruCommand, bool>
{
    private readonly IYeuCauCuTruEFRepository _yeuCauRepository;
    private readonly IUnitOfWork _unitOfWork;

    public XoaYeuCauCuTruCommandHandler(IYeuCauCuTruEFRepository yeuCauRepository, IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(XoaYeuCauCuTruCommand request, CancellationToken cancellationToken)
    {
        if (request.Ids == null || request.Ids.Count == 0)
            return Result.Failure<bool>(GeneralErrors.BadRequest("Danh sách ID không được để trống."));

        foreach (var id in request.Ids)
        {
            var yeuCau = await _yeuCauRepository.GetByIdAsync(id, cancellationToken);
            if (yeuCau != null)
            {
                _yeuCauRepository.Delete(yeuCau);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
