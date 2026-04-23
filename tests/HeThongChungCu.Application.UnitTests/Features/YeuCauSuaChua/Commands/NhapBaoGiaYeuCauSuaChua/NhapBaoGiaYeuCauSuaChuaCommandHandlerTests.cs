using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.NhapBaoGiaYeuCauSuaChua;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Exceptions;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.NhapBaoGiaYeuCauSuaChua;

public sealed class NhapBaoGiaYeuCauSuaChuaCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly NhapBaoGiaYeuCauSuaChuaCommandHandler _handler;

    public NhapBaoGiaYeuCauSuaChuaCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new NhapBaoGiaYeuCauSuaChuaCommandHandler(
            _ycscRepository,
            _canHoRepository,
            _toaNhaRepository,
            _nguoiDungRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_SetChoCuDanDuyetBaoGia_When_NotFree()
    {
        // Arrange
        var request = new NhapBaoGiaYeuCauSuaChuaCommand(1, 500000, false, "Fix broken pipe");
        var ycsc = CreateTestYcsc(request.Id);
        // Set state to DaDieuPhoi via AssignInternalStaff
        ycsc.AssignInternalStaff(1);

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.ChiPhiDuKien.Should().Be(request.ChiPhiDuKien);
        ycsc.IsMienPhi.Should().BeFalse();
        ycsc.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.ChoCuDanDuyetBaoGia);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_SetDaDuyetBaoGia_When_Free()
    {
        // Arrange
        var request = new NhapBaoGiaYeuCauSuaChuaCommand(1, 0, true, "Minor fix, no cost");
        var ycsc = CreateTestYcsc(request.Id);
        // Set state to DaDieuPhoi via AssignInternalStaff
        ycsc.AssignInternalStaff(1);

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.IsMienPhi.Should().BeTrue();
        ycsc.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.DaDuyetBaoGia);
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessException_When_InvalidState()
    {
        // Arrange
        var request = new NhapBaoGiaYeuCauSuaChuaCommand(1, 500000, false, "Note");
        var ycsc = CreateTestYcsc(request.Id); // State becomes MoiTao
        
        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken));
        exception.Message.Should().Be("Chỉ có thể nhập báo giá khi đã được điều phối hoặc đang chờ báo giá.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_YcscNotFound()
    {
        // Arrange
        var request = new NhapBaoGiaYeuCauSuaChuaCommand(1, 100, true, null);
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

        // YeuCau triggers Domain Event in constructor depending on base. Manually set ID
        typeof(Domain.Entities.YeuCauSuaChua).GetProperty(nameof(Domain.Entities.YeuCauSuaChua.Id))?.SetValue(ycsc, id);
        return ycsc;
    }
}
