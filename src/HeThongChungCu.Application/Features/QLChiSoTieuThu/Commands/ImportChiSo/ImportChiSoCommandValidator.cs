using FluentValidation;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ImportChiSo;

public class ImportChiSoCommandValidator : AbstractValidator<ImportChiSoCommand>
{
    public ImportChiSoCommandValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File Excel không được để trống.");

        RuleFor(x => x.Thang)
            .InclusiveBetween(1, 12).WithMessage("Tháng phải từ 1 đến 12.");

        RuleFor(x => x.Nam)
            .GreaterThanOrEqualTo(2000).WithMessage("Năm không hợp lệ.");

        RuleFor(x => x.NgayGhiNhan)
            .NotEmpty().WithMessage("Ngày ghi nhận không được để trống.");
    }
}
