using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.GiaoViecDoiTac;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.GiaoViecDoiTac;

public sealed class GiaoViecDoiTacCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly IDoiTacCommandRepository _doiTacRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GiaoViecDoiTacCommandHandler _handler;

    public GiaoViecDoiTacCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _doiTacRepository = CreateMock<IDoiTacCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new GiaoViecDoiTacCommandHandler(
            _ycscRepository,
            _doiTacRepository,
            _canHoRepository,
            _toaNhaRepository,
            _nguoiDungRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RequestIsValid()
    {
        // Arrange
        var request = new GiaoViecDoiTacCommand(1, 2, new List<NhanSuPartnerDTO>
        {
            new("Partner Staff", "123456789", "0912345678", "Technician", "Note")
        });
        var ycsc = CreateTestYcsc(request.Id);
        var hopDong = CreateTestHopDong(request.HopDongDoiTacId);

        _ycscRepository.GetByIdWithPersonnelAsync(request.Id, CancellationToken).Returns(ycsc);
        _doiTacRepository.GetHopDongByIdAsync(request.HopDongDoiTacId, CancellationToken).Returns(hopDong);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.HopDongDoiTacId.Should().Be(request.HopDongDoiTacId);
        ycsc.NhanSuSuaChuas.Should().HaveCount(1);
        ycsc.NhanSuSuaChuas.Should().Contain(ns => ns.HoTen == "Partner Staff");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_HopDongNotFound()
    {
        // Arrange
        var request = new GiaoViecDoiTacCommand(1, 2, new List<NhanSuPartnerDTO>());
        var ycsc = CreateTestYcsc(request.Id);

        _ycscRepository.GetByIdWithPersonnelAsync(request.Id, CancellationToken).Returns(ycsc);
        _doiTacRepository.GetHopDongByIdAsync(request.HopDongDoiTacId, CancellationToken).Returns((HopDongDoiTac?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(DoiTacErrors.NotFoundById(request.HopDongDoiTacId));
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

    private static HopDongDoiTac CreateTestHopDong(int id)
    {
        var hopDong = new HopDongDoiTac(
            doiTacId: 1,
            soHopDong: "HD001",
            ngayKy: DateTimeOffset.Now.AddMonths(-1),
            ngayHetHan: DateTimeOffset.Now.AddMonths(11),
            giaTri: 1000000,
            dichVuId: 1);

        typeof(HopDongDoiTac).GetProperty(nameof(HopDongDoiTac.Id))?.SetValue(hopDong, id);
        return hopDong;
    }
}
