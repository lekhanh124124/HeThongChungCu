using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetDichVuById;

public class GetDichVuByIdSpecification : BaseSpecification
{
    public GetDichVuByIdSpecification(int id)
        : base(null, null, null, null)
    {
        // Bộ lọc cho Dịch vụ (Root)
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("Id", FilterOperator.Equal, id);

        // Bộ lọc cho Khung giờ
        AddFilter("KhungGioIsActive", FilterOperator.Equal, true);
        AddFilter("KhungGioIsDeleted", FilterOperator.Equal, false);

        // Bộ lọc cho Bảng giá
        AddFilter("BangGiaIsActive", FilterOperator.Equal, true);
        AddFilter("BangGiaIsDeleted", FilterOperator.Equal, false);

        // Bộ lọc cho Tệp dữ liệu
        AddFilter("LoaiTepTaiLieu", FilterOperator.Equal, LoaiTepTaiLieu.MacDinh.Value);
        AddFilter("TepDuLieuIsDeleted", FilterOperator.Equal, false);

        // Bộ lọc cho Hợp đồng
        AddFilter("TrangThaiHopDongId", FilterOperator.In, new List<int> { TrangThaiHopDong.ConHieuLuc.Value, TrangThaiHopDong.SapHetHan.Value });
    }
}
