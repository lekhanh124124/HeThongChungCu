using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.Auth.Commands.Login;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.Auth.Login;

public sealed class LoginCommandHandlerTests : BaseTest
{
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly IHasherService _hasherService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _accountRepository = CreateMock<ITaiKhoanCommandRepository>();
        _userRepository = CreateMock<INguoiDungCommandRepository>();
        _hasherService = CreateMock<IHasherService>();
        _tokenService = CreateMock<ITokenService>();
        _dateTimeProvider = CreateMock<IDateTimeProvider>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new LoginCommandHandler(
            _accountRepository,
            _userRepository,
            _hasherService,
            _tokenService,
            _dateTimeProvider,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithTokens_When_CredentialsAreValid()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        var hashedPassword = "hashed_password123";
        var expectedAccessToken = "valid_access_token";
        var testTime = new DateTimeOffset(2026, 5, 25, 12, 0, 0, TimeSpan.Zero);

        var account = new TaiKhoan(null, username, "testuser@gmail.com", hashedPassword);
        
        _dateTimeProvider.UtcNow.Returns(testTime);
        _accountRepository.GetByTenDangNhapAsync(username, CancellationToken).Returns(account);
        _hasherService.VerifyPassword(password, hashedPassword).Returns(true);
        _tokenService.GenerateToken(account.Id, account.TenDangNhap, Arg.Any<System.Collections.Generic.List<string>>(), account.NguoiDungId)
            .Returns(expectedAccessToken);
        _tokenService.RefreshTokenExpiryMinutes.Returns(10);

        var command = new LoginCommand(username, password);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().Be(expectedAccessToken);
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();

        // Verify Refresh Token expires in exactly 10 minutes
        account.Tokens.Should().ContainSingle();
        var refreshToken = account.Tokens.Single();
        refreshToken.ExpiresDate.Should().Be(testTime.AddMinutes(10));
        refreshToken.IsActive.Should().BeTrue();

        // Verify repository interaction
        _accountRepository.Received(1).Update(account);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AccountDoesNotExist()
    {
        // Arrange
        var username = "nonexistent";
        _accountRepository.GetByTenDangNhapAsync(username, CancellationToken).Returns((TaiKhoan?)null);

        var command = new LoginCommand(username, "password");

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be(AuthErrors.InvalidCredentials.Code);

        // Verify no DB changes
        _accountRepository.DidNotReceive().Update(Arg.Any<TaiKhoan>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_PasswordIsIncorrect()
    {
        // Arrange
        var username = "testuser";
        var password = "wrongpassword";
        var hashedPassword = "hashed_password123";
        var account = new TaiKhoan(null, username, "testuser@gmail.com", hashedPassword);

        _accountRepository.GetByTenDangNhapAsync(username, CancellationToken).Returns(account);
        _hasherService.VerifyPassword(password, hashedPassword).Returns(false);

        var command = new LoginCommand(username, password);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be(AuthErrors.InvalidCredentials.Code);

        // Verify no DB changes
        _accountRepository.DidNotReceive().Update(Arg.Any<TaiKhoan>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken);
    }
}
