using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HuyYeuCauSuaChua;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.HuyYeuCauSuaChua;

public sealed class HuyYeuCauSuaChuaCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HuyYeuCauSuaChuaCommandHandler _handler;

    public HuyYeuCauSuaChuaCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _currentUserService = CreateCurrentUserMock();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new HuyYeuCauSuaChuaCommandHandler(
            _ycscRepository,
            _canHoRepository,
            _toaNhaRepository,
            _nguoiDungRepository,
            _currentUserService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_AdminCancels()
    {
        // Arrange
        var request = new HuyYeuCauSuaChuaCommand(1, "Reason");
        var ycsc = CreateTestYcsc(request.Id);
        _currentUserService.Roles.Returns(new List<string> { Role.Admin.Name });
        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.DaHuy);
        ycsc.LyDoHuy.Should().Be(request.LyDoHuy);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ResidentCancelsBeforeApproval()
    {
        // Arrange
        var request = new HuyYeuCauSuaChuaCommand(1, "Reason");
        var ycsc = CreateTestYcsc(request.Id); // Defaults to MoiTao
        _currentUserService.Roles.Returns(new List<string> { Role.Resident.Name });
        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.DaHuy);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ResidentCancelsAfterApproval()
    {
        // Arrange
        var request = new HuyYeuCauSuaChuaCommand(1, "Reason");
        var ycsc = CreateTestYcsc(request.Id);
        // Set state to DaDuyetBaoGia via reflection since setter is private
        typeof(Domain.Entities.YeuCauSuaChua).GetProperty(nameof(Domain.Entities.YeuCauSuaChua.TrangThaiSuaChuaId))
            ?.SetValue(ycsc, TrangThaiSuaChua.DaDuyetBaoGia);

        _currentUserService.Roles.Returns(new List<string> { Role.Resident.Name });
        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(YeuCauSuaChuaErrors.HuyForbidden);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_YcscNotFound()
    {
        // Arrange
        var request = new HuyYeuCauSuaChuaCommand(1, "Reason");
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
