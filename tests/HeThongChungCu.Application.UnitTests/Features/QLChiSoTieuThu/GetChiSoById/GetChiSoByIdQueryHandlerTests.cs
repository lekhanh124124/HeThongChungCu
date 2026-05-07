using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetChiSoById;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.GetChiSoById;

public sealed class GetChiSoByIdQueryHandlerTests : BaseTest
{
    private readonly IChiSoTieuThuQueryRepository _repository;
    private readonly GetChiSoByIdQueryHandler _handler;

    public GetChiSoByIdQueryHandlerTests()
    {
        _repository = CreateMock<IChiSoTieuThuQueryRepository>();
        _handler = new GetChiSoByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NotFound()
    {
        _repository.GetByIdAsync(Arg.Any<GetChiSoByIdSpecification>(), CancellationToken).Returns((ChiSoDetailResponse?)null);
        var query = new GetChiSoByIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be("ChiSo.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Found()
    {
        var expected = new ChiSoDetailResponse { Id = 1 };
        _repository.GetByIdAsync(Arg.Any<GetChiSoByIdSpecification>(), CancellationToken).Returns(expected);
        var query = new GetChiSoByIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
    }
}
