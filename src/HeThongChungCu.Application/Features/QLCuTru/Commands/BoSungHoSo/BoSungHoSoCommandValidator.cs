using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.BoSungHoSo;

public class BoSungHoSoCommandValidator : AbstractValidator<BoSungHoSoCommand>
{
    public BoSungHoSoCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Documents).NotEmpty().WithMessage("Phải có ít nhất một tài liệu.");

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
