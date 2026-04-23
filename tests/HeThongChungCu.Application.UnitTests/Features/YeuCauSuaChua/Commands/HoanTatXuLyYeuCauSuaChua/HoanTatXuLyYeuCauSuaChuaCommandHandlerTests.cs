using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HoanTatXuLyYeuCauSuaChua;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Exceptions;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.HoanTatXuLyYeuCauSuaChua;

public sealed class HoanTatXuLyYeuCauSuaChuaCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HoanTatXuLyYeuCauSuaChuaCommandHandler _handler;

    public HoanTatXuLyYeuCauSuaChuaCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new HoanTatXuLyYeuCauSuaChuaCommandHandler(
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
        var request = new HoanTatXuLyYeuCauSuaChuaCommand(1, "Fixed perfectly", 550000, DateTimeOffset.Now);
        var ycsc = CreateTestYcsc(request.Id);
        
        // Transition to DangXuLy
        ycsc.AssignInternalStaff(1);
        ycsc.HenLich(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(1).AddHours(1)); // State -> DaHenLich
        ycsc.BatDauXuLy(); // State -> DangXuLy
        ycsc.ClearDomainEvents(); // Clear previous events

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.DaDong);
        ycsc.KetQuaXuLy.Should().Be(request.KetQuaXuLy);
        ycsc.ChiPhiThucTe.Should().Be(request.ChiPhiThucTe);
        ycsc.NgayXuLy.Should().Be(request.NgayHoanThanh);

        // Ensure Domain Event was raised
        ycsc.DomainEvents.Should().Contain(e => e is YeuCauSuaChuaHoanTatEvent);
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessException_When_InvalidState()
    {
        // Arrange
        var request = new HoanTatXuLyYeuCauSuaChuaCommand(1, "Fixed", 100, DateTimeOffset.Now);
        var ycsc = CreateTestYcsc(request.Id); // State -> MoiTao

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken));
        exception.Message.Should().Be("Chỉ có thể hoàn tất khi đang xử lý.");
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessException_When_ResultIsEmpty()
    {
        // Arrange
        var request = new HoanTatXuLyYeuCauSuaChuaCommand(1, "", 100, DateTimeOffset.Now);
        var ycsc = CreateTestYcsc(request.Id);
        ycsc.AssignInternalStaff(1);
        ycsc.HenLich(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(1).AddHours(1));
        ycsc.BatDauXuLy();

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken));
        exception.Message.Should().Be("Cần cung cấp kết quả xử lý.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_YcscNotFound()
    {
        // Arrange
        var request = new HoanTatXuLyYeuCauSuaChuaCommand(1, "Fixed", 100, DateTimeOffset.Now);
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
