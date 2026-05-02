using FluentAssertions;
using FluentValidation.TestHelper;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.UploadChiSoImagesBatch;
using HeThongChungCu.Application.UnitTests.Abstractions;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.UploadChiSoImagesBatch;

public sealed class UploadChiSoImagesBatchCommandValidatorTests : BaseTest
{
    private readonly IZipService _zipService;
    private readonly UploadChiSoImagesBatchCommandValidator _validator;

    public UploadChiSoImagesBatchCommandValidatorTests()
    {
        _zipService = CreateMock<IZipService>();
        _validator = new UploadChiSoImagesBatchCommandValidator(_zipService);
    }

    [Fact]
    public void Validate_Should_HaveError_When_ZipStreamIsNull()
    {
        var command = new UploadChiSoImagesBatchCommand(null!, "test.zip");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ZipStream);
    }

    [Fact]
    public void Validate_Should_HaveError_When_FileNameHasWrongExtension()
    {
        var command = new UploadChiSoImagesBatchCommand(new MemoryStream(new byte[1]), "test.txt");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FileName);
    }

    [Fact]
    public void Validate_Should_HaveError_When_ZipIsInvalid()
    {
        var stream = new MemoryStream(new byte[1]);
        var command = new UploadChiSoImagesBatchCommand(stream, "test.zip");
        _zipService.IsValidZip(stream).Returns(false);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ZipStream);
    }

    [Fact]
    public void Validate_Should_NotHaveError_When_Valid()
    {
        var stream = new MemoryStream(new byte[1]);
        var command = new UploadChiSoImagesBatchCommand(stream, "test.zip");
        _zipService.IsValidZip(stream).Returns(true);

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
