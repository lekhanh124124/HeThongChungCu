using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.CuDanDuyetBaoGiaYeuCauSuaChua;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Exceptions;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.CuDanDuyetBaoGiaYeuCauSuaChua;

public sealed class CuDanDuyetBaoGiaYeuCauSuaChuaCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CuDanDuyetBaoGiaYeuCauSuaChuaCommandHandler _handler;

    public CuDanDuyetBaoGiaYeuCauSuaChuaCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new CuDanDuyetBaoGiaYeuCauSuaChuaCommandHandler(
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
        var request = new CuDanDuyetBaoGiaYeuCauSuaChuaCommand(1);
        var ycsc = CreateTestYcsc(request.Id);
        ycsc.AssignInternalStaff(1);
        ycsc.NhapBaoGia(500000, false, "Fix broken pipe"); // Sets state to ChoCuDanDuyetBaoGia

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.DaDuyetBaoGia);
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessException_When_InvalidState()
    {
        // Arrange
        var request = new CuDanDuyetBaoGiaYeuCauSuaChuaCommand(1);
        var ycsc = CreateTestYcsc(request.Id); // State becomes MoiTao
        
        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken));
        exception.Message.Should().Be("Hiện tại không có báo giá nào đang chờ bạn duyệt.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_YcscNotFound()
    {
        // Arrange
        var request = new CuDanDuyetBaoGiaYeuCauSuaChuaCommand(1);
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
