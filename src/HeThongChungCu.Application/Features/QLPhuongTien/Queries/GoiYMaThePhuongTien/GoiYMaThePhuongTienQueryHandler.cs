using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GoiYMaThePhuongTien;

public class GoiYMaThePhuongTienQueryHandler : IQueryHandler<GoiYMaThePhuongTienQuery, string>
{
    private readonly IPhuongTienEFRepository _phuongTienRepository;

    public GoiYMaThePhuongTienQueryHandler(IPhuongTienEFRepository phuongTienRepository)
    {
        _phuongTienRepository = phuongTienRepository;
    }

    public async Task<Result<string>> Handle(GoiYMaThePhuongTienQuery request, CancellationToken cancellationToken)
    {
        var phuongTien = await _phuongTienRepository.GetPhuongTienByIdAsync(request.PhuongTienId, cancellationToken);
        if (phuongTien == null)
            return Result.Failure<string>(PhuongTienErrors.NotFound);

        var lastTheId = await _phuongTienRepository.GetMaxThePhuongTienIdAsync(cancellationToken);
        int nextTheId = lastTheId + 1;

        string formattedPhuongTienId = FormatWithRandom(request.PhuongTienId);
        string formattedNextTheId = FormatWithRandom(nextTheId);

        string suggestMaThe = $"CARD-V-{formattedPhuongTienId}{formattedNextTheId}";

        return Result.Success(suggestMaThe);
    }

    private string FormatWithRandom(int id)
    {
        string idStr = id.ToString();
        string s = idStr.PadLeft(4, '0');
        char[] chars = s.ToCharArray();
        Random random = new Random();
        int idLength = idStr.Length;
        
        // Loop through the "padding" part (the leading zeros)
        for (int i = 0; i < 4 - idLength; i++)
        {
            chars[i] = (char)('0' + random.Next(0, 10));
        }
        
        return new string(chars);
    }
}
