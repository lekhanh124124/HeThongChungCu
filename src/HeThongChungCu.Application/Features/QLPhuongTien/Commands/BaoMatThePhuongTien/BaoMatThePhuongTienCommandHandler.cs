using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.BaoMatThePhuongTien;

public class BaoMatThePhuongTienCommandHandler : ICommandHandler<BaoMatThePhuongTienCommand, bool>
{
    private readonly IPhuongTienCommandRepository _phuongTienCommandRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BaoMatThePhuongTienCommandHandler(
        IPhuongTienCommandRepository phuongTienCommandRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _phuongTienCommandRepository = phuongTienCommandRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<bool>> Handle(BaoMatThePhuongTienCommand request, CancellationToken cancellationToken)
    {
        var phuongTiens = await _phuongTienCommandRepository.GetPhuongTiensByTheIdsAsync(request.TheIds, cancellationToken);
        var now = _dateTimeProvider.Now.DateTime;

        if (!phuongTiens.Any())
            return PhuongTienErrors.NotFound;

        foreach (var theId in request.TheIds)
        {
            var phuongTien = phuongTiens.FirstOrDefault(x => x.ThePhuongTiens.Any(t => t.Id == theId));
            if (phuongTien != null)
            {
                phuongTien.BaoMatThe(theId, now);
                _phuongTienCommandRepository.Update(phuongTien);
            }
        }

        return true;
    }
}
