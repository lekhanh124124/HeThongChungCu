using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.QuetHoaDonQuaHan;

public class QuetHoaDonQuaHanCommandHandler : ICommandHandler<QuetHoaDonQuaHanCommand, int>
{
    private readonly IHoaDonCommandRepository _hoaDonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QuetHoaDonQuaHanCommandHandler(
        IHoaDonCommandRepository hoaDonRepository,
        IUnitOfWork unitOfWork)
    {
        _hoaDonRepository = hoaDonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(QuetHoaDonQuaHanCommand request, CancellationToken cancellationToken)
    {
        var today = DateTimeOffset.Now;

        var overdueInvoices = await _hoaDonRepository.GetPendingPastDueInvoicesAsync(today, cancellationToken);
        if (overdueInvoices.Count == 0)
            return Result.Success(0);

        foreach (var invoice in overdueInvoices)
        {
            invoice.UpdateStatus(TrangThaiHoaDon.QuaHan);
            _hoaDonRepository.Update(invoice);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(overdueInvoices.Count);
    }
}
