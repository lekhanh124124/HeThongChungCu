namespace HeThongChungCu.Application.Features.CanHo.Commands.CreateCanHo;

public class CreateCanHoCommandValidator : AbstractValidator<CreateCanHoCommand>
{
    public CreateCanHoCommandValidator()
    {

        RuleFor(x => x.MaCanHo)
            .NotEmpty().WithMessage("Mã căn hộ không được để trống.")
            .MaximumLength(20).WithMessage("Mã căn hộ không được vượt quá 20 ký tự.");

        RuleFor(x => x.TenCanHo)
            .NotEmpty().WithMessage("Tên căn hộ không được để trống.")
            .MaximumLength(100).WithMessage("Tên căn hộ không được vượt quá 100 ký tự.");

        RuleFor(x => x.DienTich)
            .GreaterThan(0).WithMessage("Diện tích phải lớn hơn 0.");

        RuleFor(x => x.TangId)
            .GreaterThan(0).WithMessage("ID tầng không hợp lệ.");

        RuleFor(x => x.SoPhongNgu)
            .GreaterThanOrEqualTo(0).WithMessage("Số phòng ngủ không được âm.");

        RuleFor(x => x.SoPhongTam)
            .GreaterThanOrEqualTo(0).WithMessage("Số phòng tắm không được âm.");

        RuleFor(x => x.LoaiCanHoId)
            .Must(id => LoaiCanHo.GetAll().Any(g => g.Value == id))
            .WithMessage($"Loại căn hộ không hợp lệ. Các giá trị hợp lệ: " +
                         $"{string.Join(", ", LoaiCanHo.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");

    }
}
