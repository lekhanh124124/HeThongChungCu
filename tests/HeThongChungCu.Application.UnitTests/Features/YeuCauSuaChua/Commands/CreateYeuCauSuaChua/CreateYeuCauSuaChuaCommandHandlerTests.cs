using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.CreateYeuCauSuaChua;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.ValueObjects;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.CreateYeuCauSuaChua;

public sealed class CreateYeuCauSuaChuaCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateYeuCauSuaChuaCommandHandler _handler;

    public CreateYeuCauSuaChuaCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _tepTaiLieuRepository = CreateMock<ITepTaiLieuCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _currentUserService = CreateCurrentUserMock();
        _dateTimeProvider = CreateDateTimeMock();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new CreateYeuCauSuaChuaCommandHandler(
            _ycscRepository,
            _canHoRepository,
            _toaNhaRepository,
            _tepTaiLieuRepository,
            _nguoiDungRepository,
            _currentUserService,
            _dateTimeProvider,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RequestIsValid()
    {
        // Arrange
        var request = CreateValidRequest();
        var canHo = CreateTestCanHo(request.CanHoId);

        _canHoRepository.GetByIdAsync(request.CanHoId, CancellationToken)
            .Returns(canHo);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_AddEntityToRepository_When_RequestIsValid()
    {
        // Arrange
        var request = CreateValidRequest();
        var canHo = CreateTestCanHo(request.CanHoId);

        _canHoRepository.GetByIdAsync(request.CanHoId, CancellationToken)
            .Returns(canHo);

        // Act
        await _handler.Handle(request, CancellationToken);

        // Assert
        await _ycscRepository.Received(1).AddAsync(
            Arg.Is<Domain.Entities.YeuCauSuaChua>(yc => yc.CanHoId == request.CanHoId && yc.NoiDung == request.NoiDung),
            CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChangesAsync_When_RequestIsValid()
    {
        // Arrange
        var request = CreateValidRequest();
        var canHo = CreateTestCanHo(request.CanHoId);

        _canHoRepository.GetByIdAsync(request.CanHoId, CancellationToken)
            .Returns(canHo);

        // Act
        await _handler.Handle(request, CancellationToken);

        // Assert
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CanHoNotFound()
    {
        // Arrange
        var request = CreateValidRequest();
        _canHoRepository.GetByIdAsync(request.CanHoId, CancellationToken)
            .Returns((CanHo?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(CanHoErrors.NotFoundById(request.CanHoId));
    }

    [Fact]
    public async Task Handle_Should_ReturnCorrectResponseData()
    {
        // Arrange
        var request = CreateValidRequest();
        var canHo = CreateTestCanHo(request.CanHoId);
        var toaNha = CreateTestToaNha();
        var user = new NguoiDung("Test", "User", DateTimeOffset.Now.AddYears(-30), GioiTinh.Nam, "123 Street", "123456789", "0912345678");

        _canHoRepository.GetByIdAsync(request.CanHoId, CancellationToken).Returns(canHo);
        _toaNhaRepository.GetToaNhaByTangIdAsync(canHo.TangId, CancellationToken).Returns(toaNha);
        _nguoiDungRepository.GetByIdAsync(_currentUserService.UserId!.Value, CancellationToken).Returns(user);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TenCanHo.Should().Be(canHo.MaCanHo);
        result.Value.TenToaNha.Should().Be(toaNha.MaToaNha);
        result.Value.TenNguoiGui.Should().Be(user.HoTen);
    }

    private static CreateYeuCauSuaChuaCommand CreateValidRequest()
    {
        return new CreateYeuCauSuaChuaCommand(
            CanHoId: 1,
            PhamViId: 1, // Riêng
            LoaiSuCoId: 1, // Điện
            MucDoUuTienDeXuatId: 1, // Thường
            NoiDung: "Test repair request",
            MoTaViTri: "Living room",
            DanhSachTepIds: new List<int>());
    }

    private static CanHo CreateTestCanHo(int id)
    {
        var canHo = CanHo.Create(
            tangId: 10,
            maCanHo: "A101",
            tenCanHo: "Apartment A101",
            dienTich: 75,
            soPhongNgu: 2,
            soPhongTam: 2,
            loaiCanHoId: LoaiCanHo.Standard,
            tinhTrangCanHoId: TrangThaiCanHo.CoCuDan);

        // Set ID via reflection if there is no public setter
        typeof(CanHo).GetProperty(nameof(CanHo.Id))?.SetValue(canHo, id);
        return canHo;
    }

    private static ToaNha CreateTestToaNha()
    {
        var toaNha = new ToaNha(
            maToaNha: "B_A",
            tenToaNha: "Building A",
            block: "A",
            diaChi: "123 Street",
            moTa: "Note",
            trangThaiToaNhaId: TrangThaiToaNha.DangHoatDong);

        // Add floor to building - this will use the internal constructor of Tang
        toaNha.AddTang("L10", "Level 10", LoaiTang.TangLau);
        
        // Ensure the added floor has the expected ID
        var tang = toaNha.Tangs.First();
        typeof(Tang).GetProperty(nameof(Tang.Id))?.SetValue(tang, 10);
        
        return toaNha;
    }
}
