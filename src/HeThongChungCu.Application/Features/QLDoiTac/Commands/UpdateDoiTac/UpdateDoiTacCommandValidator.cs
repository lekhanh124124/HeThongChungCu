using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.UpdateDoiTac;

public class UpdateDoiTacCommandValidator : AbstractValidator<UpdateDoiTacCommand>
{
    public UpdateDoiTacCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithErrorCode("Validation.NotEmpty").WithMessage("ID không được để trống.");

        RuleFor(x => x.TenDoiTac)
            .NotEmpty().WithErrorCode("Validation.NotEmpty").WithMessage("Tên đơn vị cung cấp không được để trống.")
            .MaximumLength(100).WithErrorCode("Validation.MaxLength").WithMessage("Tên đơn vị cung cấp không được vượt quá 100 ký tự.");

        RuleFor(x => x.TenCongTy)
            .MaximumLength(200).WithErrorCode("Validation.MaxLength").WithMessage("Tên công ty không được vượt quá 200 ký tự.");

        RuleFor(x => x.NguoiDaiDien)
            .MaximumLength(100).WithErrorCode("Validation.MaxLength").WithMessage("Người đại diện không được vượt quá 100 ký tự.");

        RuleFor(x => x.SoGiayPhepKD)
            .MaximumLength(50).WithErrorCode("Validation.MaxLength").WithMessage("Số giấy phép kinh doanh không được vượt quá 50 ký tự.");

        RuleFor(x => x.MaSoThue)
            .MaximumLength(50).WithErrorCode("Validation.MaxLength").WithMessage("Mã số thuế không được vượt quá 50 ký tự.");

        RuleFor(x => x.SoDienThoai)
            .MaximumLength(20).WithErrorCode("Validation.MaxLength").WithMessage("Số điện thoại không được vượt quá 20 ký tự.");

        RuleFor(x => x.Email)
            .MaximumLength(100).WithErrorCode("Validation.MaxLength").WithMessage("Email không được vượt quá 100 ký tự.");

        RuleFor(x => x.GhiChu)
            .MaximumLength(1000).WithErrorCode("Validation.MaxLength").WithMessage("Ghi chú không được vượt quá 1000 ký tự.");
    }
}
