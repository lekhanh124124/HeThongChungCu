using FluentAssertions;
using FluentValidation.TestHelper;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.RecordChiSoBatch;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.RecordChiSoBatch;

public sealed class RecordChiSoBatchCommandValidatorTests : BaseTest
{
    private readonly RecordChiSoBatchCommandValidator _validator;

    public RecordChiSoBatchCommandValidatorTests()
    {
        _validator = new RecordChiSoBatchCommandValidator();
    }

    [Fact]
    public void Validate_Should_HaveError_When_ItemsEmpty()
    {
        var command = new RecordChiSoBatchCommand(new List<ChiSoBatchItemDto>(), 5, 2024, DateTimeOffset.Now);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Validate_Should_HaveError_When_ThangInvalid()
    {
        var command = new RecordChiSoBatchCommand(new List<ChiSoBatchItemDto> { new ChiSoBatchItemDto() }, 13, 2024, DateTimeOffset.Now);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Thang);
    }

    [Fact]
    public void Validate_Should_HaveError_When_ChiSoMoiLessThanChiSoCu()
    {
        var item = new ChiSoBatchItemDto { CanHoId = 1, DichVuId = 1, ChiSoCu = 20, ChiSoMoi = 10 };
        var command = new RecordChiSoBatchCommand(new List<ChiSoBatchItemDto> { item }, 5, 2024, DateTimeOffset.Now);
        var result = _validator.TestValidate(command);
        
        // Assert that at least one error corresponds to the Items array child rule
        result.Errors.Should().Contain(e => e.ErrorMessage == "Chỉ số mới không được nhỏ hơn chỉ số cũ.");
    }
}
