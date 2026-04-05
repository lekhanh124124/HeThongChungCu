using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.CanHo.Queries.GoiYMaCanHo;

public class GoiYMaCanHoQueryHandler : IQueryHandler<GoiYMaCanHoQuery, string>
{
    private readonly ICodeGeneratorService _codeGeneratorService;

    public GoiYMaCanHoQueryHandler(ICodeGeneratorService codeGeneratorService)
    {
        _codeGeneratorService = codeGeneratorService;
    }

    public async Task<Result<string>> Handle(GoiYMaCanHoQuery request, CancellationToken cancellationToken)
    {
        var maCanHo = await _codeGeneratorService.GenerateMaCanHoAsync(request.TangId, cancellationToken);
        return Result.Success(maCanHo);
    }
}
