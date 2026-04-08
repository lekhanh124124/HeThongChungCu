using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateKhungGioDichVu;

public class CreateKhungGioDichVuCommandValidator : AbstractValidator<CreateKhungGioDichVuCommand>
{
    public CreateKhungGioDichVuCommandValidator()
    {
        RuleFor(x => x.DichVuId)
            .NotEmpty().WithMessage(DichVuErrors.DichVuIdRange.Description);

        RuleFor(x => x.TenKhungGio)
            .NotEmpty().WithMessage(DichVuErrors.TenKhungGioNotEmpty.Description)
            .MaximumLength(100).WithMessage(DichVuErrors.TenKhungGioMaxLength.Description);

        RuleFor(x => x.GioBatDau)
            .NotEmpty().WithMessage(DichVuErrors.GioBatDauNotEmpty.Description);

        RuleFor(x => x.GioKetThuc)
            .NotEmpty().WithMessage(DichVuErrors.GioKetThucNotEmpty.Description)
            .GreaterThan(x => x.GioBatDau).WithMessage(DichVuErrors.GioKetThucGreaterThanBatDau.Description);

        RuleFor(x => x.NgayTrongTuan)
            .InclusiveBetween(0, 6).When(x => x.NgayTrongTuan.HasValue)
            .WithMessage(DichVuErrors.NgayTrongTuanRange.Description);
    }
}
