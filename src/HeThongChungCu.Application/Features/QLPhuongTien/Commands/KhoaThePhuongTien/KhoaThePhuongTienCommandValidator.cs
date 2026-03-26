namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaThePhuongTien
{
    public class KhoaThePhuongTienCommandValidator : AbstractValidator<KhoaThePhuongTienCommand>
    {
        public KhoaThePhuongTienCommandValidator()
        {
            RuleFor(x => x.TheIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("Danh sách mã thẻ không được để trống.");
        }
    }
}
