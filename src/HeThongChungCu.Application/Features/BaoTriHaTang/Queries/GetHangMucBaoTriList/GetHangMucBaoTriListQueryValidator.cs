namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriList;

public class GetHangMucBaoTriListQueryValidator : AbstractValidator<GetHangMucBaoTriListQuery>
{
    public GetHangMucBaoTriListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Số trang phải lớn hơn hoặc bằng 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Kích thước trang phải lớn hơn hoặc bằng 1.");
    }
}
