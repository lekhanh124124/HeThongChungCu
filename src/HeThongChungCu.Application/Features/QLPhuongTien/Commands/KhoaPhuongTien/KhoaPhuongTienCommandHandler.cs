using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaPhuongTien;

internal sealed class KhoaPhuongTienCommandHandler : ICommandHandler<KhoaPhuongTienCommand, bool>
{
    private readonly IPhuongTienEFRepository _phuongTienEFRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public KhoaPhuongTienCommandHandler(
        IPhuongTienEFRepository phuongTienEFRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _phuongTienEFRepository = phuongTienEFRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(KhoaPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var phuongTiens = await _phuongTienEFRepository.GetPhuongTiensByIdsAsync(request.PhuongTienIds, cancellationToken);
        var now = _dateTimeProvider.Now.DateTime;

        if (phuongTiens.Count == 0)
        {
            return Result.Failure<bool>(PhuongTienErrors.NotFound);
        }

        foreach (var phuongTien in phuongTiens)
        {
            phuongTien.Khoa(now);
            _phuongTienEFRepository.Update(phuongTien);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
