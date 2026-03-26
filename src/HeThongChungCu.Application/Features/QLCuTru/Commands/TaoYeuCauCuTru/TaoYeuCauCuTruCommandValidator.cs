using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoYeuCauCuTru;

public class TaoYeuCauCuTruCommandValidator : AbstractValidator<TaoYeuCauCuTruCommand>
{
    public TaoYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.CanHoId).NotEmpty();
        RuleFor(x => x.LoaiYeuCauId).NotEmpty();
        
        When(x => x.LoaiYeuCauId == 1, () => // AddMember
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.PhoneNumber).MaximumLength(20);
            RuleFor(x => x.Dob).NotEmpty();
            RuleFor(x => x.GioiTinhId).NotEmpty();
            RuleFor(x => x.LoaiQuanHeId).NotEmpty();
        });

        When(x => x.LoaiYeuCauId > 1, () => // Update/Remove/ChangeHead
        {
            RuleFor(x => x.QuanHeCuTruId).NotEmpty();
        });

        When(x => x.LoaiYeuCauId == 2, () => // UpdateRelationship
        {
            RuleFor(x => x.NewLoaiQuanHeId).NotEmpty();
        });

        RuleForEach(x => x.TaiLieuCuTrus).ChildRules(attachment =>
        {
            attachment.RuleFor(a => a.LoaiGiayToId).NotEmpty();
            attachment.RuleFor(a => a.SoGiayTo).NotEmpty().MaximumLength(100);
            attachment.RuleFor(a => a.FileIds).NotEmpty().WithMessage("Mỗi tài liệu phải có ít nhất một tệp tin đính kèm.");
            attachment.RuleForEach(a => a.FileIds).GreaterThan(0).WithMessage("ID của tệp tin không hợp lệ.");
        });
    }
}
