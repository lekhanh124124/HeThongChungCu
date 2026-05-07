namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriById;

public class GetHangMucBaoTriByIdQueryValidator : AbstractValidator<GetHangMucBaoTriByIdQuery>
{
    public GetHangMucBaoTriByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID hạng mục bảo trì không được để trống.");
    }
}
