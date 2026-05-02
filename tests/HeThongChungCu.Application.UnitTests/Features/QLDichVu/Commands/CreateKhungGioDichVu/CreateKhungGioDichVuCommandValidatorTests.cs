using FluentValidation.TestHelper;
using HeThongChungCu.Application.Features.QLDichVu.Commands.CreateKhungGioDichVu;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLDichVu.Commands.CreateKhungGioDichVu;

public class CreateKhungGioDichVuCommandValidatorTests
{
    private readonly CreateKhungGioDichVuCommandValidator _validator;

    public CreateKhungGioDichVuCommandValidatorTests()
    {
        _validator = new CreateKhungGioDichVuCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_DichVuIdIsZero()
    {
        var command = new CreateKhungGioDichVuCommand(0, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), "Sáng");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DichVuId);
    }

    [Fact]
    public void Should_NotHaveError_When_GioBatDauIsMidnight()
    {
        // Bug: If GioBatDau is 00:00:00 (TimeSpan.Zero), NotEmpty() will flag it as error.
        // We should allow 00:00:00 as a valid start time.
        var command = new CreateKhungGioDichVuCommand(1, TimeSpan.Zero, new TimeSpan(6, 0, 0), "Nửa đêm");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.GioBatDau);
    }

    [Fact]
    public void Should_HaveError_When_GioKetThuc_BeforeOrEqual_GioBatDau()
    {
        var command = new CreateKhungGioDichVuCommand(1, new TimeSpan(10, 0, 0), new TimeSpan(8, 0, 0), "Lỗi");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.GioKetThuc);
    }

    [Fact]
    public void Should_HaveError_When_NgayTrongTuan_IsOutOfRange()
    {
        var command = new CreateKhungGioDichVuCommand(1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), "Sáng", 7);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.NgayTrongTuan);
    }
}
