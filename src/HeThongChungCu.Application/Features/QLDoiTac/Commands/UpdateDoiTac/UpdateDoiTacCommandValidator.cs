using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.UpdateDoiTac;

public class UpdateDoiTacCommandValidator : AbstractValidator<UpdateDoiTacCommand>
{
    public UpdateDoiTacCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithErrorCode(DoiTacErrors.IdNotEmpty.Code).WithMessage(DoiTacErrors.IdNotEmpty.Description);

        RuleFor(x => x.TenDoiTac)
            .NotEmpty().WithErrorCode(DoiTacErrors.TenDoiTacNotEmpty.Code).WithMessage(DoiTacErrors.TenDoiTacNotEmpty.Description)
            .MaximumLength(100).WithErrorCode(DoiTacErrors.TenDoiTacMaxLength.Code).WithMessage(DoiTacErrors.TenDoiTacMaxLength.Description);

        RuleFor(x => x.TenCongTy)
            .MaximumLength(200).WithErrorCode(DoiTacErrors.TenCongTyMaxLength.Code).WithMessage(DoiTacErrors.TenCongTyMaxLength.Description);

        RuleFor(x => x.NguoiDaiDien)
            .MaximumLength(100).WithErrorCode(DoiTacErrors.NguoiDaiDienMaxLength.Code).WithMessage(DoiTacErrors.NguoiDaiDienMaxLength.Description);

        RuleFor(x => x.SoGiayPhepKD)
            .MaximumLength(50).WithErrorCode(DoiTacErrors.SoGiayPhepKDMaxLength.Code).WithMessage(DoiTacErrors.SoGiayPhepKDMaxLength.Description);

        RuleFor(x => x.MaSoThue)
            .MaximumLength(50).WithErrorCode(DoiTacErrors.MaSoThueMaxLength.Code).WithMessage(DoiTacErrors.MaSoThueMaxLength.Description);

        RuleFor(x => x.SoDienThoai)
            .MaximumLength(20).WithErrorCode(DoiTacErrors.SoDienThoaiMaxLength.Code).WithMessage(DoiTacErrors.SoDienThoaiMaxLength.Description);

        RuleFor(x => x.Email)
            .MaximumLength(100).WithErrorCode(DoiTacErrors.EmailMaxLength.Code).WithMessage(DoiTacErrors.EmailMaxLength.Description);

        RuleFor(x => x.GhiChu)
            .MaximumLength(1000).WithErrorCode(DoiTacErrors.GhiChuMaxLength.Code).WithMessage(DoiTacErrors.GhiChuMaxLength.Description);
    }
}
