using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;

public class GetYeuCauCuTruByIdQueryHandler : IQueryHandler<GetYeuCauCuTruByIdQuery, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruEFRepository _yeuCauRepository;

    public GetYeuCauCuTruByIdQueryHandler(IYeuCauCuTruEFRepository yeuCauRepository)
    {
        _yeuCauRepository = yeuCauRepository;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(GetYeuCauCuTruByIdQuery request, CancellationToken cancellationToken)
    {
        var yeuCau = await _yeuCauRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (yeuCau == null)
            return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.NotFoundById(request.RequestId));

        // Use the repository or just leave MaCanHo empty if we want full decoupling.
        // Actually, for query handlers, it's often better to use Dapper if we need joins.
        // But for consistency with the existing EF-based implementation, I'll just set it to string.Empty for now or find another way.
        // I'll check for CanHo repo.
        
        var response = new YeuCauCuTruResponse
        {
            Id = yeuCau.Id,
            CanHoId = yeuCau.CanHoId,
            LoaiYeuCauId = yeuCau.LoaiYeuCauId.Value,
            TenLoaiYeuCau = yeuCau.LoaiYeuCauId.Name,
            TrangThaiId = yeuCau.TrangThaiId.Value,
            TenTrangThai = yeuCau.TrangThaiId.Name,
            TargetQuanHeCuTruId = yeuCau.QuanHeCuTruId,
            
            YeuCauTen = yeuCau.YeuCauTen,
            YeuCauHo = yeuCau.YeuCauHo,
            YeuCauNgaySinh = yeuCau.YeuCauNgaySinh,
            YeuCauGioiTinhId = yeuCau.YeuCauGioiTinhId,
            YeuCauSoDienThoai = yeuCau.YeuCauSoDienThoai,
            YeuCauLoaiQuanHeId = yeuCau.YeuCauLoaiQuanHeId,
            
            NoiDung = yeuCau.NoiDung,
            LyDo = yeuCau.LyDo,
            
            CreatedAt = yeuCau.CreatedAt,
            NgayXuLy = yeuCau.NgayXuLy,
            NguoiXuLyId = yeuCau.NguoiXuLyId,
            
            Documents = yeuCau.YeuCauTaiLieuCuTrus.Select(d => new TaiLieuResponse
            {
                Id = d.Id,
                LoaiGiayToId = d.LoaiGiayToId.Value,
                TenLoaiGiayTo = d.LoaiGiayToId.Name,
                SoGiayTo = d.SoGiayTo,
                NgayPhatHanh = d.NgayPhatHanh,
                TargetTaiLieuCuTruId = d.TaiLieuCuTruId,
                Files = d.Files.Select(f => new TepTaiLieuResponse(f.Id, f.FileUrl, f.FileName, f.ContentType)).ToList()
            }).ToList()
        };

        return response;
    }
}
