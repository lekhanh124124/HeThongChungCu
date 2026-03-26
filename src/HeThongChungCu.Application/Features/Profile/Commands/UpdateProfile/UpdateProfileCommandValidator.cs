using FluentValidation;

namespace HeThongChungCu.Application.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public UpdateProfileCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không đúng định dạng.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Họ và tên đệm không được để trống.")
            .MaximumLength(50).WithMessage("Họ và tên đệm không quá 50 ký tự.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Tên không được để trống.")
            .MaximumLength(50).WithMessage("Tên không quá 50 ký tự.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d{10,11}$")
            .WithMessage("Số điện thoại không hợp lệ.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Dob)
            .NotEmpty().WithMessage("Ngày sinh không được để trống.")
            .LessThan(_dateTimeProvider.Now.Date).WithMessage("Ngày sinh không được lớn hơn ngày hiện tại.");

        RuleFor(x => x.GioiTinhId)
            .GreaterThan(0).WithMessage("Giới tính không hợp lệ.");
    }
}
