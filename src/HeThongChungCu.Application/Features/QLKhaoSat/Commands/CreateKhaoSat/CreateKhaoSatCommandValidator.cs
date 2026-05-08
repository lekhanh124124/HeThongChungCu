using FluentValidation;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.CreateKhaoSat;

public class CreateKhaoSatCommandValidator : AbstractValidator<CreateKhaoSatCommand>
{
    public CreateKhaoSatCommandValidator()
    {
        RuleFor(x => x.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề đợt khảo sát không được để trống.")
            .MaximumLength(200).WithMessage("Tiêu đề không được vượt quá 200 ký tự.");

        RuleFor(x => x.MoTa)
            .NotEmpty().WithMessage("Mô tả đợt khảo sát không được để trống.")
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

        RuleFor(x => x.NgayBatDau)
            .NotEmpty().WithMessage("Ngày bắt đầu không được để trống.");

        RuleFor(x => x.NgayKetThuc)
            .NotEmpty().WithMessage("Ngày kết thúc không được để trống.")
            .GreaterThan(x => x.NgayBatDau).WithMessage("Ngày kết thúc phải diễn ra sau ngày bắt đầu.");

        RuleFor(x => x.TyleThamGiaToiThieu)
            .InclusiveBetween(0.0m, 100.0m).WithMessage("Tỷ lệ tham gia tối thiểu phải từ 0% đến 100%.");

        RuleFor(x => x.TyLeDongYToiThieu)
            .InclusiveBetween(0.0m, 100.0m).WithMessage("Tỷ lệ đồng ý tối thiểu phải từ 0% đến 100%.");

        RuleFor(x => x.CauHois)
            .NotEmpty().WithMessage("Đợt khảo sát/bầu cử phải chứa ít nhất 1 câu hỏi.");

        RuleForEach(x => x.CauHois).ChildRules(q =>
        {
            q.RuleFor(c => c.NoiDungCauHoi)
                .NotEmpty().WithMessage("Nội dung câu hỏi không được để trống.");

            q.RuleFor(c => c.LuaChons)
                .NotEmpty().WithMessage("Mỗi câu hỏi phải có ít nhất 2 đáp án lựa chọn.")
                .Must(list => list != null && list.Count >= 2).WithMessage("Mỗi câu hỏi phải chứa ít nhất 2 phương án lựa chọn.");

            q.RuleForEach(c => c.LuaChons).ChildRules(o =>
            {
                o.RuleFor(x => x.NoiDungLuaChon)
                    .NotEmpty().WithMessage("Nội dung phương án lựa chọn không được để trống.");
            });
        });
    }
}
