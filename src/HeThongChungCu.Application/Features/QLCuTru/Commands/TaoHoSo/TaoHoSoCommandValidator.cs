using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoHoSo;

public class TaoHoSoCommandValidator : AbstractValidator<TaoHoSoCommand>
{
    public TaoHoSoCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Dob).NotEmpty().LessThan(DateTime.UtcNow);
        RuleFor(x => x.GioiTinhId).Must(id => id > 0);
        RuleFor(x => x.DiaChi).NotEmpty().MaximumLength(200);

        RuleFor(x => x.Documents).NotEmpty().When(x => x.Documents != null);

        RuleForEach(x => x.Documents).ChildRules(doc =>
        {
            doc.RuleFor(d => d.LoaiGiayToId)
                .Must(id => LoaiGiayTo.GetAll().Any(g => g.Value == id))
                .WithMessage($"Loại giấy tờ không hợp lệ. Các giá trị hợp lệ: " +
                             $"{string.Join(", ", LoaiGiayTo.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");

            doc.RuleFor(d => d.SoGiayTo).NotEmpty().MaximumLength(100);
            doc.RuleFor(d => d.FileIds).NotEmpty().WithMessage("Mỗi tài liệu phải có ít nhất một tệp tin đính kèm.");
            doc.RuleForEach(d => d.FileIds).GreaterThan(0).WithMessage("ID của tệp tin không hợp lệ.");
        });
    }
}
