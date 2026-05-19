using HeThongChungCu.Application.Common.Behaviors;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLSystem.Commands.RestoreBackup;
using HeThongChungCu.Application.UnitTests.Abstractions;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Behaviors;

public class MaintenanceBehaviorTests : BaseTest
{
    private readonly IMaintenanceService _maintenanceService;
    private readonly ILogger<MaintenanceBehavior<IRequest<object>, object>> _logger;

    public MaintenanceBehaviorTests()
    {
        _maintenanceService = CreateMock<IMaintenanceService>();
        _logger = CreateMock<ILogger<MaintenanceBehavior<IRequest<object>, object>>>();
    }

    [Fact]
    public async Task Handle_WhenMaintenanceInactive_ShouldCallNext()
    {
        // Arrange
        _maintenanceService.IsMaintenanceActive().Returns(false); // Bảo trì tắt

        var behavior = new MaintenanceBehavior<FakeCommand, Result<int>>(_maintenanceService, CreateMock<ILogger<MaintenanceBehavior<FakeCommand, Result<int>>>>());
        var command = new FakeCommand();
        var nextCalled = false;
        RequestHandlerDelegate<Result<int>> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Success(42));
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken);

        // Assert
        Assert.True(nextCalled);
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public async Task Handle_WhenMaintenanceActive_AndCommandIsRestoreBackupCommand_ShouldCallNext()
    {
        // Arrange
        _maintenanceService.IsMaintenanceActive().Returns(true); // Bảo trì bật nhưng lệnh là Restore

        var behavior = new MaintenanceBehavior<RestoreBackupCommand, Result<bool>>(_maintenanceService, CreateMock<ILogger<MaintenanceBehavior<RestoreBackupCommand, Result<bool>>>>());
        var command = new RestoreBackupCommand(1);
        var nextCalled = false;
        RequestHandlerDelegate<Result<bool>> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Success(true));
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken);

        // Assert
        Assert.True(nextCalled);
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_WhenMaintenanceActive_AndReturnsGenericResult_ShouldReturnFailureResult()
    {
        // Arrange
        _maintenanceService.IsMaintenanceActive().Returns(true); // Bảo trì bật

        var behavior = new MaintenanceBehavior<FakeCommand, Result<int>>(_maintenanceService, CreateMock<ILogger<MaintenanceBehavior<FakeCommand, Result<int>>>>());
        var command = new FakeCommand();
        var nextCalled = false;
        RequestHandlerDelegate<Result<int>> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Success(42));
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken);

        // Assert
        Assert.False(nextCalled); // Hoàn toàn không gọi logic Handler
        Assert.True(result.IsFailure);
        Assert.Equal("System.Maintenance", result.Errors.First().Code);
    }

    [Fact]
    public async Task Handle_WhenMaintenanceActive_AndReturnsNonGenericResult_ShouldReturnFailureResult()
    {
        // Arrange
        _maintenanceService.IsMaintenanceActive().Returns(true); // Bảo trì bật

        var behavior = new MaintenanceBehavior<FakeResultCommand, Result>(_maintenanceService, CreateMock<ILogger<MaintenanceBehavior<FakeResultCommand, Result>>>());
        var command = new FakeResultCommand();
        var nextCalled = false;
        RequestHandlerDelegate<Result> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Success());
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken);

        // Assert
        Assert.False(nextCalled);
        Assert.True(result.IsFailure);
        Assert.Equal("System.Maintenance", result.Errors.First().Code);
    }

    [Fact]
    public async Task Handle_WhenMaintenanceActive_AndReturnsRawType_ShouldThrowBusinessException()
    {
        // Arrange
        _maintenanceService.IsMaintenanceActive().Returns(true); // Bảo trì bật

        var behavior = new MaintenanceBehavior<FakeRawCommand, int>(_maintenanceService, CreateMock<ILogger<MaintenanceBehavior<FakeRawCommand, int>>>());
        var command = new FakeRawCommand();
        RequestHandlerDelegate<int> next = (ct) => Task.FromResult(100);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => behavior.Handle(command, next, CancellationToken));
        Assert.Equal("System.Maintenance", exception.ErrorCode);
    }
}

// Lớp giả lập cho test
public class FakeCommand : IRequest<Result<int>> { }
public class FakeResultCommand : IRequest<Result> { }
public class FakeRawCommand : IRequest<int> { }
