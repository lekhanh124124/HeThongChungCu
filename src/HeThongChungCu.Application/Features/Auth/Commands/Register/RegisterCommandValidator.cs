namespace HeThongChungCu.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public RegisterCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không hợp lệ.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Tên không được để trống.")
            .MaximumLength(100).WithMessage("Tên không được vượt quá 100 ký tự.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Họ không được để trống.")
            .MaximumLength(100).WithMessage("Họ không được vượt quá 100 ký tự.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống.")
            .MaximumLength(50).WithMessage("Tên đăng nhập không được vượt quá 50 ký tự.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.")
            .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự.");

        RuleFor(x => x.IdCard)
            .NotEmpty().WithMessage("Số CCCD không được để trống.")
            .MaximumLength(20).WithMessage("Số CCCD không được vượt quá 20 ký tự.");

        RuleFor(x => x.Dob)
            .NotEmpty().WithMessage("Ngày sinh không được để trống.")
            .LessThan(_dateTimeProvider.Now.Date).WithMessage("Ngày sinh phải là ngày trong quá khứ.");

        RuleFor(x => x.GioiTinhId)
            .Must(id => GioiTinh.GetAll().Any(g => g.Value == id))
            .WithMessage($"Giới tính không hợp lệ. Các giá trị hợp lệ: " +
                         $"{string.Join(", ", GioiTinh.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
    }
}
