using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences;
using HeThongChungCu.Application.Features.QLDichVu.Commands.CreateKhungGioDichVu;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLDichVu.Commands.CreateKhungGioDichVu;

public class CreateKhungGioDichVuCommandHandlerTests
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateKhungGioDichVuCommandHandler _handler;

    public CreateKhungGioDichVuCommandHandlerTests()
    {
        _dichVuCommandRepository = Substitute.For<IDichVuCommandRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateKhungGioDichVuCommandHandler(_dichVuCommandRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_AddingValidKhungGio()
    {
        // Arrange
        var dichVu = new DichVu("DV01", "Dich Vu 1", LoaiDichVu.TienIch, "Gio");
        
        _dichVuCommandRepository.GetByIdWithKhungGiosAsync(1, Arg.Any<CancellationToken>())
            .Returns(dichVu);

        var command = new CreateKhungGioDichVuCommand(
            1,
            new TimeSpan(8, 0, 0),
            new TimeSpan(10, 0, 0),
            "Sáng",
            0 // Chủ nhật
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TenKhungGio.Should().Be("Sáng");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_KhungGioOverlaps()
    {
        // Arrange
        var dichVu = new DichVu("DV01", "Dich Vu 1", LoaiDichVu.TienIch, "Gio");
        
        // Add existing KhungGio: 8h-10h
        var addResult = dichVu.AddKhungGio(new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), "Sáng");
        
        // Cần kích hoạt để có thể xét overlap (vì overlap chỉ check IsActive = true)
        dichVu.ActivateKhungGio(addResult.Value.Id);

        _dichVuCommandRepository.GetByIdWithKhungGiosAsync(1, Arg.Any<CancellationToken>())
            .Returns(dichVu);

        // Lệnh thêm khung giờ 9h-11h (Bị trùng)
        var command = new CreateKhungGioDichVuCommand(
            1,
            new TimeSpan(9, 0, 0),
            new TimeSpan(11, 0, 0),
            "Sáng muộn"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.First().Code.Should().Be("DichVu.KhungGioOverlap");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_DichVuNotFound()
    {
        // Arrange
        _dichVuCommandRepository.GetByIdWithKhungGiosAsync(99, Arg.Any<CancellationToken>())
            .Returns((DichVu?)null);

        var command = new CreateKhungGioDichVuCommand(
            99,
            new TimeSpan(8, 0, 0),
            new TimeSpan(10, 0, 0),
            "Sáng"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.First().Code.Should().Be("DichVu.NotFound");
    }
}
