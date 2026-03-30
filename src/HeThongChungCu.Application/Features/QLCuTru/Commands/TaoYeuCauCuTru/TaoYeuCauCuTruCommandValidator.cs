using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoYeuCauCuTru;

public class TaoYeuCauCuTruCommandValidator : AbstractValidator<TaoYeuCauCuTruCommand>
{
    public TaoYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty()
            .WithMessage("CanHoId không được để trống.");

        RuleFor(x => x.LoaiYeuCauId)
            .NotEmpty()
            .WithMessage("LoaiYeuCauId không được để trống.")
            .Must(id => LoaiYeuCau.GetAll().Any(g => g.Value == id))
            .WithMessage($"Loại yêu cầu không hợp lệ. Các giá trị hợp lệ: " +
                             $"{string.Join(", ", LoaiYeuCau.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");


        When(x => x.LoaiYeuCauId == LoaiYeuCau.Them.Value, () => // AddMember
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Tên không được để trống.")
                .MaximumLength(50)
                .WithMessage("Tên không được vượt quá 50 ký tự.");
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Họ không được để trống.")
                .MaximumLength(50)
                .WithMessage("Họ không được vượt quá 50 ký tự.");
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .WithMessage("Số điện thoại không được vượt quá 20 ký tự.");
            RuleFor(x => x.Dob)
                .NotEmpty()
                .WithMessage("Ngày sinh không được để trống.");
            RuleFor(x => x.GioiTinhId)
                .NotEmpty()
                .WithMessage("Giới tính không được để trống.");
            RuleFor(x => x.CCCD)
                .MaximumLength(50)
                .WithMessage("CCCD không được vượt quá 50 ký tự.");
            RuleFor(x => x.DiaChi)
                .MaximumLength(200)
                .WithMessage("Địa chỉ không được vượt quá 200 ký tự.");
            RuleFor(x => x.LoaiQuanHeId)
                .NotEmpty()
                .WithMessage("Loại quan hệ không được để trống.");
        });

        When(x => x.LoaiYeuCauId != LoaiYeuCau.Them.Value, () => // Update/Remove/ChangeHead
        {
            RuleFor(x => x.TargetQuanHeCuTruId)
                .NotEmpty()
                .WithMessage("TargetQuanHeCuTruId không được để trống.");
        });

        When(x => x.LoaiYeuCauId == LoaiYeuCau.Sua.Value, () => // UpdateRelationship
        {
            RuleFor(x => x.LoaiQuanHeId)
                .NotEmpty()
                .WithMessage("Loại quan hệ không được để trống.");
        });

        RuleForEach(x => x.TaiLieuCuTrus).ChildRules(attachment =>
        {
            attachment.RuleFor(a => a.LoaiGiayToId)
                .NotEmpty()
                .WithMessage("Loại giấy tờ không được để trống.");
            attachment.RuleFor(a => a.SoGiayTo)
                .NotEmpty()
                .WithMessage("Số giấy tờ không được để trống.")
                .MaximumLength(100)
                .WithMessage("Số giấy tờ không được vượt quá 100 ký tự.");
            attachment.RuleFor(a => a.FileIds)
                .NotEmpty()
                .WithMessage("Mỗi tài liệu phải có ít nhất một tệp tin đính kèm.");
            attachment.RuleForEach(a => a.FileIds)
                .GreaterThan(0)
                .WithMessage("ID của tệp tin không hợp lệ.");
        });
    }
}
