using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.BoSungNhanSuDoiTac;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.YeuCauSuaChua.Commands.BoSungNhanSuDoiTac;

public sealed class BoSungNhanSuDoiTacCommandHandlerTests : BaseTest
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BoSungNhanSuDoiTacCommandHandler _handler;

    public BoSungNhanSuDoiTacCommandHandlerTests()
    {
        _ycscRepository = CreateMock<IYeuCauSuaChuaCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _toaNhaRepository = CreateMock<IToaNhaCommandRepository>();
        _nguoiDungRepository = CreateMock<INguoiDungCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new BoSungNhanSuDoiTacCommandHandler(
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
        var request = new BoSungNhanSuDoiTacCommand(1, new List<NhanSuPartnerDTO>
        {
            new("Additional Staff", "987654321", "0987654321", "Worker", "More help")
        });
        var ycsc = CreateTestYcsc(request.Id);
        // Assign partner first
        ycsc.AssignPartner(2);

        _ycscRepository.GetByIdWithPersonnelAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ycsc.NhanSuSuaChuas.Should().Contain(ns => ns.HoTen == "Additional Staff");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoPartnerAssigned()
    {
        // Arrange
        var request = new BoSungNhanSuDoiTacCommand(1, new List<NhanSuPartnerDTO>());
        var ycsc = CreateTestYcsc(request.Id);

        _ycscRepository.GetByIdWithPersonnelAsync(request.Id, CancellationToken).Returns(ycsc);

        // Act
        var result = await _handler.Handle(request, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "YeuCauSuaChua.NoPartnerAssigned");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_YcscNotFound()
    {
        // Arrange
        var request = new BoSungNhanSuDoiTacCommand(1, new List<NhanSuPartnerDTO>());
        _ycscRepository.GetByIdWithPersonnelAsync(request.Id, CancellationToken).Returns((Domain.Entities.YeuCauSuaChua?)null);

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
