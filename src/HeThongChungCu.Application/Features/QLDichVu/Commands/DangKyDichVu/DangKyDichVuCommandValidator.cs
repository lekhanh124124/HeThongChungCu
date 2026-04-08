using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DangKyDichVu;

public class DangKyDichVuCommandValidator : AbstractValidator<DangKyDichVuCommand>
{
    public DangKyDichVuCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty().WithMessage(DichVuErrors.CanHoIdNotEmpty.Description);

        RuleFor(x => x.DichVuId)
            .NotEmpty().WithMessage(DichVuErrors.NotFound.Description);

        RuleFor(x => x.SoLuong)
            .GreaterThan(0).WithMessage(DichVuErrors.SoLuongPositive.Description);
        
        RuleFor(x => x.NgaySuDung)
            .NotEmpty().WithMessage(DichVuErrors.NgaySuDungNotEmpty.Description);
    }
}

