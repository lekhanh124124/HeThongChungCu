using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoHoSo;

public class TaoHoSoCommandValidator : AbstractValidator<TaoHoSoCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public TaoHoSoCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Họ không được để trống.")
            .MaximumLength(50).WithMessage("Họ không được vượt quá 50 ký tự.");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Tên không được để trống.")
            .MaximumLength(50).WithMessage("Tên không được vượt quá 50 ký tự.");
        RuleFor(x => x.Dob)
            .NotEmpty().WithMessage("Ngày sinh không được để trống.")
            .LessThan(_dateTimeProvider.Now.DateTime).WithMessage("Ngày sinh không được là ngày trong tương lai.");
        RuleFor(x => x.GioiTinhId)
            .NotEmpty().WithMessage("Giới tính không được để trống.")
            .Must(id => GioiTinh.GetAll().Any(g => g.Value == id))
            .WithMessage($"Giới tính không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", GioiTinh.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
        RuleFor(x => x.DiaChi)
            .MaximumLength(200).WithMessage("Địa chỉ không được vượt quá 200 ký tự.");

        RuleFor(x => x.TaiLieuCuTrus)
            .NotEmpty().WithMessage("Tệp tin đính kèm không được để trống.")
            .When(x => x.TaiLieuCuTrus != null);

        RuleForEach(x => x.TaiLieuCuTrus).ChildRules(doc =>
        {
            doc.RuleFor(d => d.LoaiGiayToId)
                .NotEmpty().WithMessage("Giá trị Giấy tờ phải nằm trong khoảng từ 1 đến 2147483647.")
                .Must(id => LoaiGiayTo.GetAll().Any(g => g.Value == id))
                .WithMessage($"Loại giấy tờ không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiGiayTo.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");

            doc.RuleFor(d => d.SoGiayTo)
                .NotEmpty().WithMessage("Số giấy tờ không được để trống.")
                .MaximumLength(100).WithMessage("Số giấy tờ không được vượt quá 100 ký tự.");
            doc.RuleFor(d => d.FileIds)
                .NotEmpty().WithMessage("Tệp tin đính kèm không được để trống.");
            doc.RuleForEach(d => d.FileIds)
                .GreaterThan(0).WithMessage("Giá trị Tệp tin phải nằm trong khoảng từ 1 đến 2147483647.");
        });
    }
}
