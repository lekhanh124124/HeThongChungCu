using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.BatDauXuLyYeuCauSuaChua;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Exceptions;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.BatDauXuLyYeuCauSuaChua;

public sealed class BatDauXuLyYeuCauSuaChuaCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BatDauXuLyYeuCauSuaChuaCommandHandler _handler;

    public BatDauXuLyYeuCauSuaChuaCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new BatDauXuLyYeuCauSuaChuaCommandHandler(
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
        var request = new BatDauXuLyYeuCauSuaChuaCommand(1);
        var ycsc = CreateTestYcsc(request.Id);
        ycsc.AssignInternalStaff(1);
        ycsc.HenLich(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(1).AddHours(1)); // State -> DaHenLich

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.DangXuLy);
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessException_When_NotAssigned()
    {
        // Arrange
        var request = new BatDauXuLyYeuCauSuaChuaCommand(1);
        var ycsc = CreateTestYcsc(request.Id); // State -> MoiTao, unassigned
        
        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken));
        exception.Message.Should().Be("Yêu cầu chưa được điều phối nhân sự hoặc đối tác để bắt đầu xử lý.");
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessException_When_InvalidState()
    {
        // Arrange
        var request = new BatDauXuLyYeuCauSuaChuaCommand(1);
        var ycsc = CreateTestYcsc(request.Id);
        ycsc.AssignInternalStaff(1); // State -> DaDieuPhoi, not DaHenLich or DaDuyetBaoGia

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken));
        exception.Message.Should().Be("Chỉ có thể bắt đầu xử lý khi đã duyệt báo giá hoặc đã hẹn lịch.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_YcscNotFound()
    {
        // Arrange
        var request = new BatDauXuLyYeuCauSuaChuaCommand(1);
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
