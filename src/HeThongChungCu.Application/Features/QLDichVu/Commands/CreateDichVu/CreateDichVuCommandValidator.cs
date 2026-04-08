using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateDichVu;

public class CreateDichVuCommandValidator : AbstractValidator<CreateDichVuCommand>
{
    public CreateDichVuCommandValidator()
    {
        RuleFor(x => x.MaDichVu)
            .NotEmpty().WithMessage(DichVuErrors.MaDichVuNotEmpty.Description)
            .MaximumLength(20).WithMessage(DichVuErrors.MaDichVuMaxLength.Description);

        RuleFor(x => x.TenDichVu)
            .NotEmpty().WithMessage(DichVuErrors.TenDichVuNotEmpty.Description)
            .MaximumLength(200).WithMessage(DichVuErrors.TenDichVuMaxLength.Description);

        RuleFor(x => x.LoaiDichVuId)
            .GreaterThan(0).WithMessage(DichVuErrors.InvalidType(LoaiDichVu.GetAll().Select(x => x.Name)).Description);

        RuleFor(x => x.DonViTinh)
            .NotEmpty().WithMessage(DichVuErrors.DonViTinhNotEmpty.Description)
            .MaximumLength(50).WithMessage(DichVuErrors.DonViTinhMaxLength.Description);

        RuleFor(x => x.MoTa)
            .MaximumLength(500).WithMessage(DichVuErrors.MoTaMaxLength.Description);
    }
}

