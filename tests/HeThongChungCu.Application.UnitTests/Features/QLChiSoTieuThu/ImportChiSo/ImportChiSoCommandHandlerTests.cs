using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ImportChiSo;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.ImportChiSo;

public sealed class ImportChiSoCommandHandlerTests : BaseTest
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IExcelService _excelService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ImportChiSoCommandHandler _handler;

    public ImportChiSoCommandHandlerTests()
    {
        _chiSoRepository = CreateMock<IChiSoTieuThuCommandRepository>();
        _excelService = CreateMock<IExcelService>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new ImportChiSoCommandHandler(
            _chiSoRepository,
            _excelService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ExcelDataIsEmpty()
    {
        // Arrange
        var command = new ImportChiSoCommand(new MemoryStream(), 5, 2024, DateTimeOffset.Now);
        _excelService.Import<ChiSoImportDto>(command.FileStream).Returns(new List<ChiSoImportDto>());

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be("Import.Empty");
    }

    [Fact]
    public async Task Handle_Should_Skip_When_AlreadyExistsForPeriod()
    {
        // Arrange
        var command = new ImportChiSoCommand(new MemoryStream(), 5, 2024, DateTimeOffset.Now);
        var dto = new ChiSoImportDto { CanHoId = 1, DichVuId = 1, ChiSoCu = 10, SoMoi = 20 };
        _excelService.Import<ChiSoImportDto>(command.FileStream).Returns(new List<ChiSoImportDto> { dto });
        
        var existingChiSo = ChiSoTieuThu.Create(1, 1, 0, 10, 5, 2024, DateTimeOffset.Now);
        _chiSoRepository.GetByPeriodAsync(command.Thang, command.Nam, CancellationToken).Returns(new List<ChiSoTieuThu> { existingChiSo });

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);
        await _chiSoRepository.DidNotReceive().AddRangeAsync(Arg.Any<IEnumerable<ChiSoTieuThu>>(), CancellationToken);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_Skip_When_NewIndexLessThanOldIndex()
    {
        // Arrange
        var command = new ImportChiSoCommand(new MemoryStream(), 5, 2024, DateTimeOffset.Now);
        var dto = new ChiSoImportDto { CanHoId = 1, DichVuId = 1, ChiSoCu = 20, SoMoi = 10 }; // Invalid
        _excelService.Import<ChiSoImportDto>(command.FileStream).Returns(new List<ChiSoImportDto> { dto });
        
        _chiSoRepository.GetByPeriodAsync(command.Thang, command.Nam, CancellationToken).Returns(new List<ChiSoTieuThu>());

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);
        await _chiSoRepository.DidNotReceive().AddRangeAsync(Arg.Any<IEnumerable<ChiSoTieuThu>>(), CancellationToken);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_AddEntitiesAndCallSaveChanges_When_ValidData()
    {
        // Arrange
        var command = new ImportChiSoCommand(new MemoryStream(), 5, 2024, DateTimeOffset.Now);
        var dto = new ChiSoImportDto { CanHoId = 1, DichVuId = 1, ChiSoCu = 10, SoMoi = 20 };
        _excelService.Import<ChiSoImportDto>(command.FileStream).Returns(new List<ChiSoImportDto> { dto });
        
        _chiSoRepository.GetByPeriodAsync(command.Thang, command.Nam, CancellationToken).Returns(new List<ChiSoTieuThu>());

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);
        await _chiSoRepository.Received(1).AddRangeAsync(Arg.Is<IEnumerable<ChiSoTieuThu>>(x => x.Count() == 1), CancellationToken);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }
}
