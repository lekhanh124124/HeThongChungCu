using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CapNhatDichVu;

public sealed record CapNhatDichVuCommand(
    int Id,
    string TenDichVu,
    string DonViTinh) : ICommand<bool>;

public sealed class CapNhatDichVuCommandValidator : AbstractValidator<CapNhatDichVuCommand>
{
    public CapNhatDichVuCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
        RuleFor(x => x.TenDichVu)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(200).WithMessage(ValidationErrors.MaxLength(200).Description);
        RuleFor(x => x.DonViTinh)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(50).WithMessage(ValidationErrors.MaxLength(50).Description);
    }
}

internal sealed class CapNhatDichVuCommandHandler : ICommandHandler<CapNhatDichVuCommand, bool>
{
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatDichVuCommandHandler(IDichVuCommandRepository dichVuRepository, IUnitOfWork unitOfWork)
    {
        _dichVuRepository = dichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CapNhatDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuRepository.GetByIdAsync(request.Id, cancellationToken);
        if (dichVu is null)
        {
            return Result.Failure<bool>(DichVuErrors.NotFound);
        }

        dichVu.Update(request.TenDichVu, request.DonViTinh);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
