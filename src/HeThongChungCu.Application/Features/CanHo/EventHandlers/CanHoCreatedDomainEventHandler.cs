using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.CanHo.EventHandlers;

public class CanHoCreatedDomainEventHandler : INotificationHandler<CanHoCreatedDomainEvent>
{
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IDangKyDichVuCommandRepository _dangKyDichVuRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CanHoCreatedDomainEventHandler> _logger;

    public CanHoCreatedDomainEventHandler(
        IDichVuCommandRepository dichVuRepository,
        IDangKyDichVuCommandRepository dangKyDichVuRepository,
        IUnitOfWork unitOfWork,
        ILogger<CanHoCreatedDomainEventHandler> logger)
    {
        _dichVuRepository = dichVuRepository;
        _dangKyDichVuRepository = dangKyDichVuRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task Handle(CanHoCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Apartment {CanHoId} created. Mandatory services will be handled by the Billing Engine implicitly.", notification.CanHo.Id);
        return Task.CompletedTask;
    }
}
