using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DangKyDichVuChoCanHo;

public sealed record DangKyDichVuChoCanHoCommand(
    int CanHoId,
    int DichVuId,
    DateTime NgayBatDau) : ICommand<DangKyDichVuResponse>;

public sealed class DangKyDichVuChoCanHoCommandValidator : AbstractValidator<DangKyDichVuChoCanHoCommand>
{
    public DangKyDichVuChoCanHoCommandValidator()
    {
        RuleFor(x => x.CanHoId).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
        RuleFor(x => x.DichVuId).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
        RuleFor(x => x.NgayBatDau).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
    }
}

internal sealed class DangKyDichVuChoCanHoCommandHandler : ICommandHandler<DangKyDichVuChoCanHoCommand, DangKyDichVuResponse>
{
    private readonly IDangKyDichVuCommandRepository _dangKyDichVuRepository;
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DangKyDichVuChoCanHoCommandHandler(
        IDangKyDichVuCommandRepository dangKyDichVuRepository, 
        IDichVuCommandRepository dichVuRepository, 
        IUnitOfWork unitOfWork)
    {
        _dangKyDichVuRepository = dangKyDichVuRepository;
        _dichVuRepository = dichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DangKyDichVuResponse>> Handle(DangKyDichVuChoCanHoCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuRepository.GetByIdAsync(request.DichVuId, cancellationToken);
        if (dichVu is null)
        {
            return Result.Failure<DangKyDichVuResponse>(DichVuErrors.NotFound);
        }

        var activeRegistration = await _dangKyDichVuRepository.GetActiveAsync(request.CanHoId, request.DichVuId, cancellationToken);
        if (activeRegistration is not null)
        {
            return Result.Failure<DangKyDichVuResponse>(DangKyDichVuErrors.AlreadyActive);
        }

        var registration = new DangKyDichVu(request.CanHoId, request.DichVuId, request.NgayBatDau);

        await _dangKyDichVuRepository.AddAsync(registration, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DangKyDichVuResponse(
            registration.Id,
            registration.CanHoId,
            registration.DichVuId,
            registration.NgayBatDau,
            registration.NgayKetThuc,
            registration.IsActive);
    }
}
