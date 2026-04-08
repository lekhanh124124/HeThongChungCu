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

    public async Task Handle(CanHoCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing mandatory service registration for apartment: {CanHoId}", notification.CanHo.Id);

        var mandatoryServices = await _dichVuRepository.GetActiveMandatoryServicesAsync(cancellationToken);

        if (mandatoryServices.Count == 0)
        {
            _logger.LogInformation("No mandatory services found to register.");
            return;
        }

        foreach (var service in mandatoryServices)
        {
            var exists = await _dangKyDichVuRepository.IsCanHoRegisteredActiveAsync(notification.CanHo.Id, service.Id, cancellationToken);
            if (exists)
            {
                _logger.LogWarning("Service {ServiceId} is already registered for apartment {CanHoId}. Skipping.", service.Id, notification.CanHo.Id);
                continue;
            }

            var registration = new DangKyDichVu(
                notification.CanHo.Id,
                service.Id,
                DateTimeOffset.Now,
                1 // Default quantity for mandatory services
            );

            // Update status to Active as requested by user
            registration.UpdateStatus(TrangThaiDangKy.DangSuDung);

            await _dangKyDichVuRepository.AddAsync(registration, cancellationToken);
            _logger.LogInformation("Registered mandatory service {ServiceId} for apartment {CanHoId}.", service.Id, notification.CanHo.Id);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
