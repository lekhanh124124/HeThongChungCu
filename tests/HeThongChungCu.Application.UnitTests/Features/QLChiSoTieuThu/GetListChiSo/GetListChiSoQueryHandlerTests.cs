using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetListChiSo;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.GetListChiSo;

public sealed class GetListChiSoQueryHandlerTests : BaseTest
{
    private readonly IChiSoTieuThuQueryRepository _repository;
    private readonly GetListChiSoQueryHandler _handler;

    public GetListChiSoQueryHandlerTests()
    {
        _repository = CreateMock<IChiSoTieuThuQueryRepository>();
        _handler = new GetListChiSoQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_Should_ReturnPagedResult()
    {
        var query = new GetListChiSoQuery(5, 2024, 1, 1, 1, 10, "Id", true);
        var expected = new PagedResult<ChiSoResponse> { Items = new List<ChiSoResponse>(), PagingInfo = new PagingInfo { TotalItems = 0, PageNumber = 1, PageSize = 10 } };
        
        _repository.GetListAsync(Arg.Any<GetListChiSoSpecification>(), CancellationToken).Returns(expected);

        var result = await _handler.Handle(query, CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
    }
}
