namespace HeThongChungCu.Application.Features.ChungCu.Commands.CreateCanHo;

public class CreateCanHoCommandValidator : AbstractValidator<CreateCanHoCommand>
{
    public CreateCanHoCommandValidator()
    {
        RuleFor(x => x.ToaNhaId)
            .GreaterThan(0).WithMessage("ID tòa nhà không hợp lệ.");

        RuleFor(x => x.MaCanHo)
            .NotEmpty().WithMessage("Mã căn hộ không được để trống.")
            .MaximumLength(20).WithMessage("Mã căn hộ không được vượt quá 20 ký tự.");

        RuleFor(x => x.DienTich)
            .GreaterThan(0).WithMessage("Diện tích phải lớn hơn 0.");

        RuleFor(x => x.Tang)
            .GreaterThan(0).WithMessage("Tầng phải lớn hơn 0.");

        RuleFor(x => x.SoPhongNgu)
            .GreaterThanOrEqualTo(0).WithMessage("Số phòng ngủ không được âm.");

        RuleFor(x => x.SoPhongTam)
            .GreaterThanOrEqualTo(0).WithMessage("Số phòng tắm không được âm.");

        RuleFor(x => x.TinhTrangCanHoId)
            .GreaterThan(0).WithMessage("Tình trạng căn hộ không hợp lệ.");
    }
}
