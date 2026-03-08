namespace HeThongChungCu.Application.Features.ChungCu.Commands.UpdateCanHo;

public class UpdateCanHoCommandValidator : AbstractValidator<UpdateCanHoCommand>
{
    public UpdateCanHoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID căn hộ không hợp lệ.");

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
