using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.TaoDichVu;

public sealed record TaoDichVuCommand(
    string MaDichVu,
    string TenDichVu,
    string DonViTinh) : ICommand<DichVuResponse>;

public sealed class TaoDichVuCommandValidator : AbstractValidator<TaoDichVuCommand>
{
    public TaoDichVuCommandValidator()
    {
        RuleFor(x => x.MaDichVu)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(20).WithMessage(ValidationErrors.MaxLength(20).Description);
        RuleFor(x => x.TenDichVu)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(200).WithMessage(ValidationErrors.MaxLength(200).Description);
        RuleFor(x => x.DonViTinh)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(50).WithMessage(ValidationErrors.MaxLength(50).Description);
    }
}

internal sealed class TaoDichVuCommandHandler : ICommandHandler<TaoDichVuCommand, DichVuResponse>
{
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaoDichVuCommandHandler(IDichVuCommandRepository dichVuRepository, IUnitOfWork unitOfWork)
    {
        _dichVuRepository = dichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DichVuResponse>> Handle(TaoDichVuCommand request, CancellationToken cancellationToken)
    {
        var isCodeUnique = await _dichVuRepository.MaDichVuExistsAsync(request.MaDichVu, cancellationToken);
        if (isCodeUnique)
        {
            return Result.Failure<DichVuResponse>(DichVuErrors.MaDichVuAlreadyExists);
        }

        var dichVu = new DichVu(
            request.MaDichVu,
            request.TenDichVu,
            request.DonViTinh);

        await _dichVuRepository.AddAsync(dichVu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DichVuResponse(dichVu.Id, dichVu.MaDichVu, dichVu.TenDichVu, dichVu.DonViTinh, dichVu.IsActive);
    }
}
