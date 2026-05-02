using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.UploadChiSoImagesBatch;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.UploadChiSoImagesBatch;

public sealed class UploadChiSoImagesBatchCommandHandlerTests : BaseTest
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IZipService _zipService;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UploadChiSoImagesBatchCommandHandler _handler;

    public UploadChiSoImagesBatchCommandHandlerTests()
    {
        _chiSoRepository = CreateMock<IChiSoTieuThuCommandRepository>();
        _fileStorageService = CreateMock<IFileStorageService>();
        _zipService = CreateMock<IZipService>();
        _tepTaiLieuRepository = CreateMock<ITepTaiLieuCommandRepository>();
        _dateTimeProvider = CreateMock<IDateTimeProvider>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new UploadChiSoImagesBatchCommandHandler(
            _chiSoRepository,
            _fileStorageService,
            _zipService,
            _tepTaiLieuRepository,
            _dateTimeProvider,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ZipEmpty()
    {
        // Arrange
        var command = new UploadChiSoImagesBatchCommand(new MemoryStream(), "test.zip");
        _zipService.ExtractFilesAsync(command.ZipStream, CancellationToken).Returns(new List<(string, MemoryStream)>());

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be("Zip.EmptyImages");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoMatchingChiSo()
    {
        // Arrange
        var command = new UploadChiSoImagesBatchCommand(new MemoryStream(), "test.zip");
        var extractedFiles = new List<(string, MemoryStream)> { ("MACU.jpg", new MemoryStream()) };
        _zipService.ExtractFilesAsync(command.ZipStream, CancellationToken).Returns(extractedFiles);
        _chiSoRepository.GetByMaTraCuusAsync(Arg.Any<List<string>>(), CancellationToken).Returns(new List<ChiSoTieuThu>());

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be("Zip.NoMatches");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UploadFails()
    {
        // Arrange
        var command = new UploadChiSoImagesBatchCommand(new MemoryStream(), "test.zip");
        var extractedFiles = new List<(string, MemoryStream)> { ("MACU.jpg", new MemoryStream()) };
        _zipService.ExtractFilesAsync(command.ZipStream, CancellationToken).Returns(extractedFiles);

        var chiSo = ChiSoTieuThu.Create(1, 1, 0, 10, 5, 2024, DateTimeOffset.Now, null, null, "MACU");
        _chiSoRepository.GetByMaTraCuusAsync(Arg.Any<List<string>>(), CancellationToken).Returns(new List<ChiSoTieuThu> { chiSo });

        _fileStorageService.UploadFilesAsync(Arg.Any<List<(Stream, string, string)>>(), Arg.Any<FileCategory>(), CancellationToken)
            .Returns(Result.Failure<List<string>>(new Error("Upload.Error", "Error")));

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be("Upload.Error");
    }
}
