namespace HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;

public class UpdateCanHoCommandValidator : AbstractValidator<UpdateCanHoCommand>
{
    public UpdateCanHoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID căn hộ không hợp lệ.");

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

        RuleFor(x => x.TinhTrangCanHoId)
            .Must(id => TinhTrangCanHo.GetAll().Any(g => g.Value == id))
            .WithMessage($"Tình trạng căn hộ không hợp lệ. Các giá trị hợp lệ: " +
                         $"{string.Join(", ", TinhTrangCanHo.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
    }
}
