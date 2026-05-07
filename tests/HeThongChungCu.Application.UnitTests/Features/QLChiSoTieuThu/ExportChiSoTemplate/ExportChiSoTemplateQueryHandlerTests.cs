using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.ExportChiSoTemplate;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.ExportChiSoTemplate;

public sealed class ExportChiSoTemplateQueryHandlerTests : BaseTest
{
    private readonly IChiSoTieuThuQueryRepository _queryRepository;
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IExcelService _excelService;
    private readonly ExportChiSoTemplateQueryHandler _handler;

    public ExportChiSoTemplateQueryHandlerTests()
    {
        _queryRepository = CreateMock<IChiSoTieuThuQueryRepository>();
        _dichVuRepository = CreateMock<IDichVuCommandRepository>();
        _excelService = CreateMock<IExcelService>();
        _handler = new ExportChiSoTemplateQueryHandler(_queryRepository, _dichVuRepository, _excelService);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_DataIsEmpty()
    {
        var query = new ExportChiSoTemplateQuery(1, 1, 1, 5, 2024);
        _queryRepository.GetExcelTemplateDataAsync(Arg.Any<ExportChiSoTemplateSpecification>(), CancellationToken).Returns(new List<ChiSoExcelTemplateDto>());

        var result = await _handler.Handle(query, CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be("Export.Empty");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithBytes_When_DataExists()
    {
        var query = new ExportChiSoTemplateQuery(1, 1, 1, 5, 2024);
        var data = new List<ChiSoExcelTemplateDto> { new ChiSoExcelTemplateDto() };
        _queryRepository.GetExcelTemplateDataAsync(Arg.Any<ExportChiSoTemplateSpecification>(), CancellationToken).Returns(data);
        _excelService.CreateTemplate(data, Arg.Any<string>()).Returns(new byte[] { 1, 2, 3 });

        var result = await _handler.Handle(query, CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().NotBeEmpty();
        result.Value.ContentType.Should().Contain("spreadsheetml");
    }
}
