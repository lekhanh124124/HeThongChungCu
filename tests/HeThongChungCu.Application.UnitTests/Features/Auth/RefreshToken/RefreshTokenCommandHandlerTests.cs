using System;
using System.Threading.Tasks;
using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.Auth.Commands.RefreshToken;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandlerTests : BaseTest
{
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly IHasherService _hasherService;
    private readonly ITokenService _tokenService;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _accountRepository = CreateMock<ITaiKhoanCommandRepository>();
        _userRepository = CreateMock<INguoiDungCommandRepository>();
        _hasherService = CreateMock<IHasherService>();
        _tokenService = CreateMock<ITokenService>();

        _handler = new RefreshTokenCommandHandler(
            _accountRepository,
            _userRepository,
            _hasherService,
            _tokenService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithNewAccessToken_When_RefreshTokenIsValid()
    {
        // Arrange
        var rawRefreshToken = "valid_refresh_token_string";
        var hashedToken = "hashed_refresh_token_string";
        var newAccessToken = "brand_new_access_token";

        var account = new TaiKhoan(null, "testuser", "test@gmail.com", "password_hash");
        // Expires in 10 minutes (valid)
        account.AddRefreshToken(hashedToken, DateTimeOffset.UtcNow.AddMinutes(10));

        _hasherService.HashToken(rawRefreshToken).Returns(hashedToken);
        _accountRepository.GetByRefreshTokenAsync(hashedToken, CancellationToken).Returns(account);
        _tokenService.GenerateToken(account.Id, account.TenDangNhap, Arg.Any<System.Collections.Generic.List<string>>(), account.NguoiDungId)
            .Returns(newAccessToken);

        var command = new RefreshTokenCommand(rawRefreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().Be(newAccessToken);
        result.Value.RefreshToken.Should().Be(rawRefreshToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AccountWithRefreshTokenNotFound()
    {
        // Arrange
        var rawRefreshToken = "unknown_refresh_token";
        var hashedToken = "hashed_unknown_refresh_token";

        _hasherService.HashToken(rawRefreshToken).Returns(hashedToken);
        _accountRepository.GetByRefreshTokenAsync(hashedToken, CancellationToken).Returns((TaiKhoan?)null);

        var command = new RefreshTokenCommand(rawRefreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be(AuthErrors.InvalidRefreshToken.Code);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RefreshTokenIsExpired()
    {
        // Arrange
        var rawRefreshToken = "expired_refresh_token";
        var hashedToken = "hashed_expired_refresh_token";

        var account = new TaiKhoan(null, "testuser", "test@gmail.com", "password_hash");
        // Expired 10 minutes ago
        account.AddRefreshToken(hashedToken, DateTimeOffset.UtcNow.AddMinutes(-10));

        _hasherService.HashToken(rawRefreshToken).Returns(hashedToken);
        _accountRepository.GetByRefreshTokenAsync(hashedToken, CancellationToken).Returns(account);

        var command = new RefreshTokenCommand(rawRefreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be(AuthErrors.InvalidRefreshToken.Code);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RefreshTokenIsRevoked()
    {
        // Arrange
        var rawRefreshToken = "revoked_refresh_token";
        var hashedToken = "hashed_revoked_refresh_token";

        var account = new TaiKhoan(null, "testuser", "test@gmail.com", "password_hash");
        account.AddRefreshToken(hashedToken, DateTimeOffset.UtcNow.AddMinutes(10));
        // Revoke it
        account.RevokeToken(hashedToken, DateTimeOffset.UtcNow, ReasonRevoked.ReplacedByNewToken);

        _hasherService.HashToken(rawRefreshToken).Returns(hashedToken);
        _accountRepository.GetByRefreshTokenAsync(hashedToken, CancellationToken).Returns(account);

        var command = new RefreshTokenCommand(rawRefreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be(AuthErrors.InvalidRefreshToken.Code);
    }
}
