using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TiepNhanYeuCauSuaChua;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.TiepNhanYeuCauSuaChua;

public sealed class TiepNhanYeuCauSuaChuaCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TiepNhanYeuCauSuaChuaCommandHandler _handler;

    public TiepNhanYeuCauSuaChuaCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _nhanVienRepository = CreateMock<INhanVienCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _currentUserService = CreateCurrentUserMock();
        _dateTimeProvider = CreateDateTimeMock();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new TiepNhanYeuCauSuaChuaCommandHandler(
            _ycscRepository,
            _canHoRepository,
            _toaNhaRepository,
            _nhanVienRepository,
            _nguoiDungRepository,
            _currentUserService,
            _dateTimeProvider,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RequestIsValid()
    {
        // Arrange
        var request = new TiepNhanYeuCauSuaChuaCommand(1);
        var ycsc = CreateTestYcsc(request.Id);
        var employee = CreateTestEmployee();

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);
        _nhanVienRepository.GetByUserIdAsync(_currentUserService.UserId!.Value, CancellationToken).Returns(employee);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.DaTiepNhan);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChangesAsync_When_Success()
    {
        // Arrange
        var request = new TiepNhanYeuCauSuaChuaCommand(1);
        var ycsc = CreateTestYcsc(request.Id);
        var employee = CreateTestEmployee();

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);
        _nhanVienRepository.GetByUserIdAsync(_currentUserService.UserId!.Value, CancellationToken).Returns(employee);

        // Act
        await _handler.Handle(request, CancellationToken);

        // Assert
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_YcscNotFound()
    {
        // Arrange
        var request = new TiepNhanYeuCauSuaChuaCommand(1);
        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns((Domain.Entities.YeuCauSuaChua?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(YeuCauSuaChuaErrors.NotFoundById(request.Id));
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EmployeeNotFound()
    {
        // Arrange
        var request = new TiepNhanYeuCauSuaChuaCommand(1);
        var ycsc = CreateTestYcsc(request.Id);

        _ycscRepository.GetByIdAsync(request.Id, CancellationToken).Returns(ycsc);
        _nhanVienRepository.GetByUserIdAsync(_currentUserService.UserId!.Value, CancellationToken).Returns((NhanVien?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(NhanVienErrors.NotFound);
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

    private static NhanVien CreateTestEmployee()
    {
        var employee = NhanVien.CreateNhanVien(1, LoaiNhanVien.KyThuat, "NV001", System.DateTimeOffset.Now);
        typeof(NhanVien).GetProperty(nameof(NhanVien.Id))?.SetValue(employee, 1);
        return employee;
    }
}
