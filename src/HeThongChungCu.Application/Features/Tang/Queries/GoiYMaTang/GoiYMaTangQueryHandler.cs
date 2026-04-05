using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.Tang.Queries.GoiYMaTang;

public class GoiYMaTangQueryHandler : IQueryHandler<GoiYMaTangQuery, string>
{
    private readonly ICodeGeneratorService _codeGeneratorService;

    public GoiYMaTangQueryHandler(ICodeGeneratorService codeGeneratorService)
    {
        _codeGeneratorService = codeGeneratorService;
    }

    public async Task<Result<string>> Handle(GoiYMaTangQuery request, CancellationToken cancellationToken)
    {
        var maTang = await _codeGeneratorService.GenerateMaTangAsync(request.ToaNhaId, request.LoaiTangId, cancellationToken);
        return Result.Success(maTang);
    }
}
