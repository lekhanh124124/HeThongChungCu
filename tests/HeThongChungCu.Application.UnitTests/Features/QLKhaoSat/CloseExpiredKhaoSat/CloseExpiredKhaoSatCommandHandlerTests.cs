using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLKhaoSat.Commands.CloseExpiredKhaoSat;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLKhaoSat.CloseExpiredKhaoSat;

public sealed class CloseExpiredKhaoSatCommandHandlerTests : BaseTest
{
    private readonly IKhaoSatCommandRepository _khaoSatRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CloseExpiredKhaoSatCommandHandler _handler;

    public CloseExpiredKhaoSatCommandHandlerTests()
    {
        _khaoSatRepository = CreateMock<IKhaoSatCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new CloseExpiredKhaoSatCommandHandler(_khaoSatRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithZero_When_NoExpiredCampaigns()
    {
        // Arrange
        _khaoSatRepository.GetExpiredCampaignsAsync(Arg.Any<DateTimeOffset>(), CancellationToken)
            .Returns(new List<KhaoSat>());

        var command = new CloseExpiredKhaoSatCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);

        // Verify no updates and no saves
        _khaoSatRepository.DidNotReceive().Update(Arg.Any<KhaoSat>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_CloseExpiredCampaigns_When_TheyExist()
    {
        // Arrange
        var expiredCampaign = KhaoSat.Create(
            "Campaign To Be Closed",
            "This campaign is expired",
            LoaiKhaoSat.LayYKienCuDan,
            CoCheTinhDiemBauCu.MoiCanHoMotPhieu,
            DateTimeOffset.UtcNow.AddDays(-10),
            DateTimeOffset.UtcNow.AddDays(-1)).Value;

        // Force adding question & transition to active
        expiredCampaign.ThemCauHoi("Cau 1?", true, false, ["Yes", "No"]);
        expiredCampaign.PublicCampaign();

        expiredCampaign.TrangThaiId.Should().Be(TrangThaiKhaoSat.DangDienRa);

        _khaoSatRepository.GetExpiredCampaignsAsync(Arg.Any<DateTimeOffset>(), CancellationToken)
            .Returns(new List<KhaoSat> { expiredCampaign });

        var command = new CloseExpiredKhaoSatCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);

        // Verify status changed to DaKetThuc
        expiredCampaign.TrangThaiId.Should().Be(TrangThaiKhaoSat.DaKetThuc);

        // Verify repository update and unit of work save
        _khaoSatRepository.Received(1).Update(expiredCampaign);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }
}
