using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetLatestOpenDotThanhToan;

public class GetLatestOpenDotThanhToanQueryHandler : IQueryHandler<GetLatestOpenDotThanhToanQuery, DotThanhToanDetailResponse>
{
    private readonly IDotThanhToanCommandRepository _dotRepository;

    public GetLatestOpenDotThanhToanQueryHandler(IDotThanhToanCommandRepository dotRepository)
    {
        _dotRepository = dotRepository;
    }

    public async Task<Result<DotThanhToanDetailResponse>> Handle(GetLatestOpenDotThanhToanQuery request, CancellationToken cancellationToken)
    {
        var ky = new KyThanhToan(request.Thang, request.Nam);
        var dot = await _dotRepository.GetLatestOpenByKyAsync(ky, cancellationToken);

        if (dot == null)
        {
            return Result.Failure<DotThanhToanDetailResponse>(new Error("DotThanhToan.NotFound", "Không tìm thấy đợt thanh toán mở cho kỳ này."));
        }

        return new DotThanhToanDetailResponse
        {
            Id = dot.Id,
            TenDot = dot.TenDot,
            Thang = dot.KyThanhToan.Thang,
            Nam = dot.KyThanhToan.Nam,
            TrangThaiDotThanhToanId = dot.TrangThaiDotThanhToanId.Value,
            TrangThaiDotThanhToanTen = dot.TrangThaiDotThanhToanId.Name,
            NgayPhatHanh = dot.NgayPhatHanh,
            GhiChu = dot.GhiChu
        };
    }
}
