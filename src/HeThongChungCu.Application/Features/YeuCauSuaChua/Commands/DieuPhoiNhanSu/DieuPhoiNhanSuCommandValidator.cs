using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.DieuPhoiNhanSu;

public class DieuPhoiNhanSuCommandValidator : AbstractValidator<DieuPhoiNhanSuCommand>
{
    public DieuPhoiNhanSuCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.NhanSu)
            .NotEmpty().WithMessage("Danh sách nhân sự không được để trống.");

        // Cases based on Assignment Type
        When(x => x.HopDongDoiTacId.HasValue, () =>
        {
            RuleForEach(x => x.NhanSu).ChildRules(ns =>
            {
                ns.RuleFor(x => x.NhanVienId)
                    .Null().WithMessage("Không được chọn nhân viên nội bộ khi điều phối cho đối tác.");

                ns.RuleFor(x => x.HoTen)
                    .NotEmpty().WithMessage("Họ tên nhân sự đối tác không được để trống.");

                ns.RuleFor(x => x.SoCCCD)
                    .NotEmpty().WithMessage("Số CCCD nhân sự đối tác không được để trống.");
            });
        });

        When(x => x.HopDongDoiTacId == null, () =>
        {
            RuleForEach(x => x.NhanSu).ChildRules(ns =>
            {
                ns.RuleFor(x => x.NhanVienId)
                    .NotEmpty().WithMessage("ID nhân viên nội bộ không được để trống.");

                ns.RuleFor(x => x.NhanVienId)
                    .GreaterThan(0).WithMessage("ID nhân viên phải là số dương.");
            });
        });
    }
}
