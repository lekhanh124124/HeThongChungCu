namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateHangMucBaoTri;

public class UpdateHangMucBaoTriCommandValidator : AbstractValidator<UpdateHangMucBaoTriCommand>
{
    public UpdateHangMucBaoTriCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID hạng mục bảo trì không được để trống.");

        RuleFor(x => x.TenHangMuc)
            .NotEmpty().WithMessage("Tên hạng mục bảo trì không được để trống.")
            .MaximumLength(250).WithMessage("Tên hạng mục bảo trì không được vượt quá 250 ký tự.");

        RuleFor(x => x.MoTa)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

        RuleFor(x => x.ThoiGianUocTinhPhut)
            .GreaterThan(0).WithMessage("Thời gian ước tính phải lớn hơn 0 phút.");

        RuleFor(x => x.ChiPhiUocTinh)
            .GreaterThanOrEqualTo(0).WithMessage("Chi phí ước tính phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.ChecklistTieuChuan)
            .NotEmpty().WithMessage("Danh sách checklist tiêu chuẩn không được để trống.");
    }
}
