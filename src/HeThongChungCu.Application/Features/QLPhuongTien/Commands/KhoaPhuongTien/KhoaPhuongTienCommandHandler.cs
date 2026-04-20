using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaPhuongTien;

internal sealed class KhoaPhuongTienCommandHandler : ICommandHandler<KhoaPhuongTienCommand, bool>
{
    private readonly IPhuongTienCommandRepository _phuongTienCommandRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public KhoaPhuongTienCommandHandler(
        IPhuongTienCommandRepository phuongTienCommandRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _phuongTienCommandRepository = phuongTienCommandRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(KhoaPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var phuongTiens = await _phuongTienCommandRepository.GetPhuongTiensByIdsAsync(request.PhuongTienIds, cancellationToken);
        var now = _dateTimeProvider.Now.DateTime;

        if (phuongTiens.Count == 0)
        {
            return PhuongTienErrors.NotFound;
        }

        foreach (var phuongTien in phuongTiens)
        {
            phuongTien.Khoa(now);
            _phuongTienCommandRepository.Update(phuongTien);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
