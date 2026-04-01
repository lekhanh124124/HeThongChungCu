namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaThePhuongTien
{
    public class KhoaThePhuongTienCommandValidator : AbstractValidator<KhoaThePhuongTienCommand>
    {
        public KhoaThePhuongTienCommandValidator()
        {
            RuleFor(x => x.TheIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("Danh sách mã th? không du?c d? tr?ng.");
        }
    }
}
