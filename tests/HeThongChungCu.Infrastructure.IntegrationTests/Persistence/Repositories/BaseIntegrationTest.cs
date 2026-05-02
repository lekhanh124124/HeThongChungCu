using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Infrastructure.Persistence;
using HeThongChungCu.Infrastructure.Persistence.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Respawn;
using Xunit;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories;

[Collection("Integration")]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected readonly AppDbContext DbContext;
    private readonly string _connectionString = "Server=.;Database=ChungCuThongMinh_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";
    private Respawner? _respawner;

    protected BaseIntegrationTest()
    {
        DbContext = CreateDbContext();
    }

    protected AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        var currentUserService = Substitute.For<ICurrentUserService>();
        currentUserService.UserId.Returns(1);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Now.Returns(DateTimeOffset.Now);

        var interceptor = new AuditableEntitySaveChangesInterceptor(currentUserService, dateTimeProvider);
        var publisher = Substitute.For<IPublisher>();

        return new AppDbContext(options, interceptor, publisher);
    }

    public async Task InitializeAsync()
    {
        await DbContext.Database.MigrateAsync();

        _respawner = await Respawner.CreateAsync(_connectionString, new RespawnerOptions
        {
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
        });

        await _respawner.ResetAsync(_connectionString);
    }

    public Task DisposeAsync()
    {
        DbContext.Dispose();
        return Task.CompletedTask;
    }
}
