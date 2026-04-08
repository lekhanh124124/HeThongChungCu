using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using MediatR;

namespace HeThongChungCu.Application.Features.QLDoiTac.EventHandlers;

public class DoiTacHopDongHetHanEventHandler : INotificationHandler<DoiTacHopDongHetHanEvent>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IDoiTacCommandRepository _doiTacCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DoiTacHopDongHetHanEventHandler(
        IDichVuCommandRepository dichVuCommandRepository,
        IDoiTacCommandRepository doiTacCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _doiTacCommandRepository = doiTacCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DoiTacHopDongHetHanEvent notification, CancellationToken cancellationToken)
    {
        var doiTac = await _doiTacCommandRepository.GetByIdWithHopDongsAsync(notification.DoiTacId, cancellationToken);
        if (doiTac == null) return;

        bool hasChanges = false;
        foreach (var hopDong in doiTac.HopDongs)
        {
            var services = await _dichVuCommandRepository.GetByHopDongAsync(hopDong.Id, cancellationToken);
            if (services.Any())
            {
                foreach (var service in services)
                {
                    service.SetCanhBao();
                    _dichVuCommandRepository.Update(service);
                }
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
