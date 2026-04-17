using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoYeuCauCuTru;

public class TaoYeuCauCuTruCommandValidator : AbstractValidator<TaoYeuCauCuTruCommand>
{
    public TaoYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty().WithMessage("Giá trị Căn hộ phải nằm trong khoảng từ 1 đến 2147483647.")
            .GreaterThan(0).WithMessage("Giá trị Căn hộ phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.LoaiYeuCauId)
            .NotEmpty().WithMessage("Loại yêu cầu không được để trống.")
            .Must(id => LoaiHanhDongYeuCau.GetAll().Any(g => g.Value == id))
            .WithMessage($"Loại yêu cầu không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiHanhDongYeuCau.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");


        When(x => x.LoaiYeuCauId == LoaiHanhDongYeuCau.Them.Value, () => // AddMember
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Họ không được để trống.")
                .MaximumLength(50).WithMessage("Họ không được vượt quá 50 ký tự.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Tên không được để trống.")
                .MaximumLength(50).WithMessage("Tên không được vượt quá 50 ký tự.");
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự.");
            RuleFor(x => x.Dob)
                .NotEmpty().WithMessage("Ngày sinh không được để trống.");
            RuleFor(x => x.GioiTinhId)
                .NotEmpty().WithMessage("Giới tính không được để trống.");
            RuleFor(x => x.CCCD)
                .MaximumLength(50).WithMessage("CCCD/CMND không được vượt quá 50 ký tự.");
            RuleFor(x => x.DiaChi)
                .MaximumLength(200).WithMessage("Địa chỉ không được vượt quá 200 ký tự.");
            RuleFor(x => x.LoaiQuanHeId)
                .NotEmpty().WithMessage("Giá trị Quan hệ cư trú phải nằm trong khoảng từ 1 đến 2147483647.");
        });

        When(x => x.LoaiYeuCauId != LoaiHanhDongYeuCau.Them.Value, () => // Update/Remove/ChangeHead
        {
            RuleFor(x => x.TargetQuanHeCuTruId)
                .NotEmpty().WithMessage("Giá trị Quan hệ cư trú phải nằm trong khoảng từ 1 đến 2147483647.")
                .GreaterThan(0).WithMessage("Giá trị Quan hệ cư trú phải nằm trong khoảng từ 1 đến 2147483647.");
        });

        When(x => x.LoaiYeuCauId == LoaiHanhDongYeuCau.Sua.Value, () => // UpdateRelationship
        {
            RuleFor(x => x.LoaiQuanHeId)
                .NotEmpty().WithMessage("Giá trị Quan hệ cư trú phải nằm trong khoảng từ 1 đến 2147483647.")
                .GreaterThan(0).WithMessage("Giá trị Quan hệ cư trú phải nằm trong khoảng từ 1 đến 2147483647.");
        });

        RuleForEach(x => x.TaiLieuCuTrus).ChildRules(attachment =>
        {
            attachment.RuleFor(a => a.LoaiGiayToId)
                .NotEmpty().WithMessage("Giá trị Giấy tờ phải nằm trong khoảng từ 1 đến 2147483647.")
                .GreaterThan(0).WithMessage("Giá trị Giấy tờ phải nằm trong khoảng từ 1 đến 2147483647.");
            attachment.RuleFor(a => a.FileIds)
                .NotEmpty().WithMessage("Tệp tin đính kèm không được để trống.");
            attachment.RuleForEach(a => a.FileIds)
                .GreaterThan(0).WithMessage("Giá trị Tệp tin phải nằm trong khoảng từ 1 đến 2147483647.");
        });
    }
}
