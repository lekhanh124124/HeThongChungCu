using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.ToaNha.Queries.GoiYMaToaNha;

public class GoiYMaToaNhaQueryHandler : IQueryHandler<GoiYMaToaNhaQuery, string>
{
    private readonly ICodeGeneratorService _codeGeneratorService;

    public GoiYMaToaNhaQueryHandler(ICodeGeneratorService codeGeneratorService)
    {
        _codeGeneratorService = codeGeneratorService;
    }

    public async Task<Result<string>> Handle(GoiYMaToaNhaQuery request, CancellationToken cancellationToken)
    {
        var maToaNha = await _codeGeneratorService.GenerateMaToaNhaAsync(cancellationToken);
        return maToaNha;
    }
}
