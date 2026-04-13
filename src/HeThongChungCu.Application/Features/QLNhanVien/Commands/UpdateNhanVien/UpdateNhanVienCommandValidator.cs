using FluentValidation;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLNhanVien.Commands.UpdateNhanVien;

public class UpdateNhanVienCommandValidator : AbstractValidator<UpdateNhanVienCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public UpdateNhanVienCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        RuleFor(x => x.Id)
            .NotEmpty();

        // User Profile Validation
        RuleFor(x => x.Ho)
            .NotEmpty().WithMessage(UserErrors.FirstNameNotEmpty.Description)
            .MaximumLength(50).WithMessage(UserErrors.FirstNameMaxLength.Description);

        RuleFor(x => x.Ten)
            .NotEmpty().WithMessage(UserErrors.LastNameNotEmpty.Description)
            .MaximumLength(50).WithMessage(UserErrors.LastNameMaxLength.Description);

        RuleFor(x => x.NgaySinh)
            .NotEmpty().WithMessage(UserErrors.DobNotEmpty.Description)
            .LessThan(_dateTimeProvider.UtcNow.DateTime).WithMessage(UserErrors.DobInFuture.Description);

        RuleFor(x => x.GioiTinhId)
            .Must(id => GioiTinh.GetAll().Any(g => g.Value == id))
            .WithMessage(UserErrors.InvalidGender(GioiTinh.GetAll().Select(g => g.Name)).Description);

        RuleFor(x => x.CCCD)
            .MaximumLength(50).WithMessage(UserErrors.CCCDMaxLength.Description);

        RuleFor(x => x.SoDienThoai)
            .MaximumLength(20).WithMessage(UserErrors.PhoneNumberMaxLength.Description);

        // Staff Validation
        RuleFor(x => x.LoaiNhanVienId)
            .Must(id => LoaiNhanVien.GetAll().Any(g => g.Value == id))
            .WithMessage(NhanVienErrors.LoaiNhanVienInvalid(LoaiNhanVien.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);

        RuleFor(x => x.TrangThaiNhanVienId)
            .Must(id => TrangThaiNhanVien.GetAll().Any(s => s.Value == id))
            .WithMessage(Error.InvalidType("Trạng thái nhân viên", TrangThaiNhanVien.GetAll().Select(t => t.Name)).Description);

        RuleFor(x => x.NgayVaoLam)
            .NotEmpty();
    }
}
