using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetDichVuTieuThu;

public class GetDichVuTieuThuQueryHandler : IQueryHandler<GetDichVuTieuThuQuery, List<DichVuResponse>>
{
    private readonly IDichVuCommandRepository _dichVuRepository;

    public GetDichVuTieuThuQueryHandler(IDichVuCommandRepository dichVuRepository)
    {
        _dichVuRepository = dichVuRepository;
    }

    public async Task<Result<List<DichVuResponse>>> Handle(GetDichVuTieuThuQuery request, CancellationToken cancellationToken)
    {
        // Lấy danh sách dịch vụ định kỳ đang hoạt động kèm bảng giá
        var services = await _dichVuRepository.GetActivePeriodicServicesWithPriceListsAsync(cancellationToken);

        // Lọc các dịch vụ có bảng giá lũy tiến đang áp dụng
        var now = DateTimeOffset.Now;
        var consumptionServices = services
            .Where(s =>
            {
                var activePrice = s.GetCurrentPrice(now);
                return activePrice != null && 
                       activePrice.IsDinhKy && 
                       activePrice.LoaiDinhGiaId == LoaiDinhGia.LuyTien;
            })
            .Select(s => new DichVuResponse
            {
                Id = s.Id,
                MaDichVu = s.MaDichVu,
                TenDichVu = s.TenDichVu,
                LoaiDichVuId = s.LoaiDichVuId.Value,
                LoaiDichVuTen = s.LoaiDichVuId.Name,
                DonViTinh = s.DonViTinh,
                MoTa = s.MoTa,
                IsBatBuoc = s.IsBatBuoc,
                SoLuongToiDa = s.SoLuongToiDa,
                TrangThaiDichVuId = s.TrangThaiId.Value,
                TrangThaiDichVuTen = s.TrangThaiId.Name
            })
            .ToList();

        return Result.Success(consumptionServices);
    }
}
