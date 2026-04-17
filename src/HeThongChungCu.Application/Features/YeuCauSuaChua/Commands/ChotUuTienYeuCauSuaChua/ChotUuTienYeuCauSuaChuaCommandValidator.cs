using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.ChotUuTienYeuCauSuaChua;

public class ChotUuTienYeuCauSuaChuaCommandValidator : AbstractValidator<ChotUuTienYeuCauSuaChuaCommand>
{
    public ChotUuTienYeuCauSuaChuaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.")
            .GreaterThan(0).WithMessage("ID yêu cầu không hợp lệ.");

        RuleFor(x => x.MucDoUuTienChotId)
            .NotEmpty().WithMessage("Mức độ ưu tiên chốt không được để trống.")
            .Must(id => MucDoUuTien.GetAll().Any(v => v.Value == id))
            .WithMessage(x => $"Mức độ ưu tiên không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", MucDoUuTien.GetAll().Select(v => $"{v.Value} ({v.Name})"))}.");
    }
}
