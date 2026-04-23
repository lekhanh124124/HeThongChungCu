using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.GiaoViecNoiBo;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.GiaoViecNoiBo;

public sealed class GiaoViecNoiBoCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GiaoViecNoiBoCommandHandler _handler;

    public GiaoViecNoiBoCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _nhanVienRepository = CreateMock<INhanVienCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new GiaoViecNoiBoCommandHandler(
            _ycscRepository,
            _nhanVienRepository,
            _canHoRepository,
            _toaNhaRepository,
            _nguoiDungRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RequestIsValid()
    {
        // Arrange
        var request = new GiaoViecNoiBoCommand(1, 2);
        var ycsc = CreateTestYcsc(request.Id);
        var nhanVien = CreateTestEmployee(request.NhanVienId);

        _ycscRepository.GetByIdWithPersonnelAsync(request.Id, CancellationToken).Returns(ycsc);
        _nhanVienRepository.GetByIdAsync(request.NhanVienId, CancellationToken).Returns(nhanVien);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.DaDieuPhoi);
        ycsc.NhanSuSuaChuas.Should().Contain(ns => ns.NhanVienId == request.NhanVienId);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_YcscNotFound()
    {
        // Arrange
        var request = new GiaoViecNoiBoCommand(1, 2);
        _ycscRepository.GetByIdWithPersonnelAsync(request.Id, CancellationToken).Returns((Domain.Entities.YeuCauSuaChua?)null);

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
        var request = new GiaoViecNoiBoCommand(1, 2);
        var ycsc = CreateTestYcsc(request.Id);

        _ycscRepository.GetByIdWithPersonnelAsync(request.Id, CancellationToken).Returns(ycsc);
        _nhanVienRepository.GetByIdAsync(request.NhanVienId, CancellationToken).Returns((NhanVien?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(NhanVienErrors.NotFoundById(request.NhanVienId));
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

    private static NhanVien CreateTestEmployee(int id)
    {
        var employee = NhanVien.CreateNhanVien(1, LoaiNhanVien.KyThuat, "NV001", System.DateTimeOffset.Now);
        typeof(NhanVien).GetProperty(nameof(NhanVien.Id))?.SetValue(employee, id);
        return employee;
    }
}
