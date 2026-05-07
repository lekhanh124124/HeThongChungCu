namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.DeleteHangMucBaoTri;

public class DeleteHangMucBaoTriCommandValidator : AbstractValidator<DeleteHangMucBaoTriCommand>
{
    public DeleteHangMucBaoTriCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID hạng mục bảo trì không được để trống.");
    }
}
