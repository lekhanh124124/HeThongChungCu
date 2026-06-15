using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.RecordChiSoBatch;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.RecordChiSoBatch;

public sealed class RecordChiSoBatchCommandHandlerTests : BaseTest
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RecordChiSoBatchCommandHandler _handler;

    public RecordChiSoBatchCommandHandlerTests()
    {
        _chiSoRepository = CreateMock<IChiSoTieuThuCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _dichVuRepository = CreateMock<IDichVuCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new RecordChiSoBatchCommandHandler(
            _chiSoRepository,
            _canHoRepository,
            _dichVuRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_AddEntitiesAndSaveChanges_When_ValidRequest()
    {
        // Arrange
        var item = new ChiSoBatchItemDto { CanHoId = 1, DichVuId = 1, ChiSoCu = 10, ChiSoMoi = 20 };
        var command = new RecordChiSoBatchCommand(new List<ChiSoBatchItemDto> { item }, 5, 2024, DateTimeOffset.Now);

        var canHo = CanHo.Create(1, "CH001", "CH001", 80, 2, 1, LoaiCanHo.Studio, TrangThaiCanHo.ChuaBanGiao);
        typeof(BaseEntity).GetProperty("Id")?.SetValue(canHo, 1);

        var dichVu = new DichVu("DV001", "Điện", LoaiDichVu.TienIch, "kWh");
        typeof(BaseEntity).GetProperty("Id")?.SetValue(dichVu, 1);

        _canHoRepository.GetByIdsAsync(Arg.Any<List<int>>(), CancellationToken)
            .Returns(new List<CanHo> { canHo });

        _dichVuRepository.GetByIdsAsync(Arg.Any<List<int>>(), CancellationToken)
            .Returns(new List<DichVu> { dichVu });

        _chiSoRepository.GetByPeriodAsync(5, 2024, CancellationToken)
            .Returns(new List<ChiSoTieuThu>());

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SuccessCount.Should().Be(1);
        result.Value.FailedCount.Should().Be(0);
        await _chiSoRepository.Received(1).AddRangeAsync(Arg.Is<IEnumerable<ChiSoTieuThu>>(x => x.Count() == 1), CancellationToken);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnZero_When_EmptyItems()
    {
        // Arrange
        var command = new RecordChiSoBatchCommand(new List<ChiSoBatchItemDto>(), 5, 2024, DateTimeOffset.Now);

        _canHoRepository.GetByIdsAsync(Arg.Any<List<int>>(), CancellationToken)
            .Returns(new List<CanHo>());

        _dichVuRepository.GetByIdsAsync(Arg.Any<List<int>>(), CancellationToken)
            .Returns(new List<DichVu>());

        _chiSoRepository.GetByPeriodAsync(5, 2024, CancellationToken)
            .Returns(new List<ChiSoTieuThu>());

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SuccessCount.Should().Be(0);
        result.Value.FailedCount.Should().Be(0);
        await _chiSoRepository.DidNotReceive().AddRangeAsync(Arg.Any<IEnumerable<ChiSoTieuThu>>(), CancellationToken);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken);
    }
}
