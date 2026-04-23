using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HenLichYeuCauSuaChua;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.HenLichYeuCauSuaChua;

public sealed class HenLichYeuCauSuaChuaCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HenLichYeuCauSuaChuaCommandHandler _handler;

    public HenLichYeuCauSuaChuaCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new HenLichYeuCauSuaChuaCommandHandler(
            _ycscRepository,
            _canHoRepository,
            _toaNhaRepository,
            _nguoiDungRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RequestIsValid()
    {
        // Arrange
        var henTu = DateTimeOffset.Now.AddDays(1);
        var henDen = henTu.AddHours(2);
        var request = new HenLichYeuCauSuaChuaCommand(1, henTu, henDen);
        var ycsc = CreateTestYcsc(request.Id);

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.HenTu.Should().Be(henTu);
        ycsc.HenDen.Should().Be(henDen);
        ycsc.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.DaHenLich);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChangesAsync_When_Success()
    {
        // Arrange
        var henTu = DateTimeOffset.Now.AddDays(1);
        var henDen = henTu.AddHours(2);
        var request = new HenLichYeuCauSuaChuaCommand(1, henTu, henDen);
        var ycsc = CreateTestYcsc(request.Id);

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        await _handler.Handle(request, CancellationToken);

        // Assert
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_YcscNotFound()
    {
        // Arrange
        var henTu = DateTimeOffset.Now.AddDays(1);
        var henDen = henTu.AddHours(2);
        var request = new HenLichYeuCauSuaChuaCommand(1, henTu, henDen);
        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns((Domain.Entities.YeuCauSuaChua?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(YeuCauSuaChuaErrors.NotFoundById(request.Id));
    }

    private static Domain.Entities.YeuCauSuaChua CreateTestYcsc(int id)
    {
        var ycsc = Domain.Entities.YeuCauSuaChua.Create(
            canHoId: 1,
            phamVi: PhamViSuaChua.TrongCanHo,
            loaiSuCo: LoaiSuCoKyThuat.Dien,
            mucDoUuTienDeXuat: MucDoUuTien.Thuong,
            noiDung: "Test",
            moTaViTri: "Test");

        typeof(Domain.Entities.YeuCauSuaChua).GetProperty(nameof(Domain.Entities.YeuCauSuaChua.Id))?.SetValue(ycsc, id);
        return ycsc;
    }
}
