using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using NSubstitute;

namespace HeThongChungCu.Application.UnitTests.Abstractions;

public abstract class BaseTest
{
    protected static CancellationToken CancellationToken => CancellationToken.None;

    protected static T CreateMock<T>() where T : class
    {
        return Substitute.For<T>();
    }

    protected static ICurrentUserService CreateCurrentUserMock(int? userId = 1)
    {
        var mock = Substitute.For<ICurrentUserService>();
        mock.UserId.Returns(userId);
        return mock;
    }

    protected static IDateTimeProvider CreateDateTimeMock()
    {
        var mock = Substitute.For<IDateTimeProvider>();
        mock.Now.Returns(DateTimeOffset.Now);
        return mock;
    }
}
