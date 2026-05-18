using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLKhaoSat.Commands.XacNhanBieuQuyet;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLKhaoSat.XacNhanBieuQuyet;

public sealed class XacNhanBieuQuyetCommandHandlerTests : BaseTest
{
    private readonly IKhaoSatCommandRepository _khaoSatRepository;
    private readonly IBieuQuyetCuDanCommandRepository _bieuQuyetRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly XacNhanBieuQuyetCommandHandler _handler;

    public XacNhanBieuQuyetCommandHandlerTests()
    {
        _khaoSatRepository = CreateMock<IKhaoSatCommandRepository>();
        _bieuQuyetRepository = CreateMock<IBieuQuyetCuDanCommandRepository>();
        _canHoRepository = CreateMock<ICanHoCommandRepository>();
        _memoryCache = CreateMock<IMemoryCache>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new XacNhanBieuQuyetCommandHandler(
            _khaoSatRepository,
            _bieuQuyetRepository,
            _canHoRepository,
            _memoryCache,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OTPMissingOrMismatched()
    {
        // Arrange
        var command = new XacNhanBieuQuyetCommand
        {
            KhaoSatId = 1,
            CanHoId = 1,
            OtpCode = "123456",
            TraLois = [new ChiTietLuaChonDto { LuaChonId = 1 }]
        };

        // Mock OTP cache returns false
        object? cachedOtp = null;
        _memoryCache.TryGetValue($"OTP_KhaoSat_{command.KhaoSatId}_{command.CanHoId}", out Arg.Any<object?>())
            .Returns(x => {
                x[1] = cachedOtp;
                return false;
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be(KhaoSatErrors.InvalidOTP.Code);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AlreadyVoted()
    {
        // Arrange
        var command = new XacNhanBieuQuyetCommand
        {
            KhaoSatId = 1,
            CanHoId = 1,
            OtpCode = "123456",
            TraLois = [new ChiTietLuaChonDto { LuaChonId = 1 }]
        };

        // Mock OTP matches
        object? cachedOtp = "123456";
        _memoryCache.TryGetValue($"OTP_KhaoSat_{command.KhaoSatId}_{command.CanHoId}", out Arg.Any<object?>())
            .Returns(x => {
                x[1] = cachedOtp;
                return true;
            });

        // Mock already voted
        _bieuQuyetRepository.HasResidentVotedAsync(command.KhaoSatId, command.CanHoId, CancellationToken)
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be(KhaoSatErrors.AlreadyVoted.Code);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_KhaoSatNotFound()
    {
        // Arrange
        var command = new XacNhanBieuQuyetCommand
        {
            KhaoSatId = 1,
            CanHoId = 1,
            OtpCode = "123456",
            TraLois = [new ChiTietLuaChonDto { LuaChonId = 1 }]
        };

        object? cachedOtp = "123456";
        _memoryCache.TryGetValue($"OTP_KhaoSat_{command.KhaoSatId}_{command.CanHoId}", out Arg.Any<object?>())
            .Returns(x => {
                x[1] = cachedOtp;
                return true;
            });

        _bieuQuyetRepository.HasResidentVotedAsync(command.KhaoSatId, command.CanHoId, CancellationToken)
            .Returns(false);

        _khaoSatRepository.GetByIdAsync(command.KhaoSatId, CancellationToken)
            .Returns((KhaoSat?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be(KhaoSatErrors.NotFound.Code);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_AllValidationsSucceed()
    {
        // Arrange
        var command = new XacNhanBieuQuyetCommand
        {
            KhaoSatId = 1,
            CanHoId = 22,
            OtpCode = "888888",
            TraLois = [new ChiTietLuaChonDto { LuaChonId = 1 }]
        };

        // Mock OTP Cache
        object? cachedOtp = "888888";
        _memoryCache.TryGetValue($"OTP_KhaoSat_{command.KhaoSatId}_{command.CanHoId}", out Arg.Any<object?>())
            .Returns(x => {
                x[1] = cachedOtp;
                return true;
            });

        _bieuQuyetRepository.HasResidentVotedAsync(command.KhaoSatId, command.CanHoId, CancellationToken)
            .Returns(false);

        // Mock KhaoSat campaign
        var khaoSat = KhaoSat.Create(
            "Khao Sat Thu",
            "Mo Ta Khảo Sát",
            LoaiKhaoSat.BieuQuyetNghiQuyet,
            CoCheTinhDiemBauCu.TheoDienTichSoHuu,
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(5)).Value;

        // Force transition to Published
        khaoSat.ThemCauHoi("Cau hoi 1?", true, false, ["Dong y", "Khong dong y"]);
        khaoSat.PublicCampaign();
        _khaoSatRepository.GetByIdAsync(command.KhaoSatId, CancellationToken).Returns(khaoSat);

        // Mock CanHo apartment (for area weight)
        var canHo = CanHo.Create(
            1, 
            "A1-02", 
            "A1-02", 
            75.5m, 
            2, 
            2, 
            LoaiCanHo.Standard, 
            TrangThaiCanHo.DaBanGiao);
        _canHoRepository.GetByIdAsync(command.CanHoId, CancellationToken).Returns(canHo);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        // Verify vote added with proper area weight
        await _bieuQuyetRepository.Received(1).AddAsync(Arg.Is<BieuQuyetCuDan>(v =>
            v.KhaoSatId == command.KhaoSatId &&
            v.CanHoId == command.CanHoId &&
            v.TrongSoBieuQuyet == 75.5m &&
            v.IsOtpVerified == true), CancellationToken);

        // Verify cache removed to prevent OTP replay
        _memoryCache.Received(1).Remove($"OTP_KhaoSat_{command.KhaoSatId}_{command.CanHoId}");

        // Verify unit of work saved changes
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }
}
