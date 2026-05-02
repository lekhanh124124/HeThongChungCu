using FluentAssertions;
using FluentValidation.TestHelper;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ImportChiSo;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.ImportChiSo;

public sealed class ImportChiSoCommandValidatorTests : BaseTest
{
    private readonly ImportChiSoCommandValidator _validator;

    public ImportChiSoCommandValidatorTests()
    {
        _validator = new ImportChiSoCommandValidator();
    }

    [Fact]
    public void Validate_Should_HaveError_When_FileStreamIsNull()
    {
        // Arrange
        var command = new ImportChiSoCommand(null!, 5, 2024, DateTimeOffset.Now);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FileStream);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Validate_Should_HaveError_When_ThangIsInvalid(int thang)
    {
        // Arrange
        var command = new ImportChiSoCommand(new MemoryStream(), thang, 2024, DateTimeOffset.Now);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Thang);
    }

    [Fact]
    public void Validate_Should_HaveError_When_NamIsInvalid()
    {
        // Arrange
        var command = new ImportChiSoCommand(new MemoryStream(), 5, 1999, DateTimeOffset.Now);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Nam);
    }

    [Fact]
    public void Validate_Should_HaveError_When_NgayGhiNhanIsEmpty()
    {
        // Arrange
        var command = new ImportChiSoCommand(new MemoryStream(), 5, 2024, default);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgayGhiNhan);
    }

    [Fact]
    public void Validate_Should_NotHaveError_When_AllFieldsAreValid()
    {
        // Arrange
        var command = new ImportChiSoCommand(new MemoryStream(), 5, 2024, DateTimeOffset.Now);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
