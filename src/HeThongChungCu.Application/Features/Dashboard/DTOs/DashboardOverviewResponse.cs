namespace HeThongChungCu.Application.Features.Dashboard.DTOs;

public record ResidentCardDto(
    int CuDanMoiTrongThang,
    int ChiSoBienDong,
    int TongTamTru,
    int TongTamVang
);

public record FeedbackCardDto(
    int SoLuongChuaXuLy,
    int SoLuongDangXuLy,
    int SoLuongHoanThanh,
    bool CoPhanAnhKhan
);

public record FinanceCardDto(
    decimal TongNoPhi,
    double TyLeThanhToan
);

public record VehicleCardDto(
    int TongXeDangKy,
    int SoLuongXeMay,
    int SoLuongOTo,
    int SoLuongXeKhac
);

public record RevenueByCategoryDto(
    string TenLoaiPhi,
    decimal SoTien,
    double TyLePhanTram
);

public record UtilityBookingDto(
    string TenTienIch,
    int LuotDungHomNay,
    int LuotDungTrongThang
);

public record MaintenanceSummaryDto(
    int LichBaoTriSapToi,
    int CongViecKT_DangXuLy,
    int SuCoQuaHan
);

public record RecentActivityDto(
    string TenNguoiThucHien,
    string AnhDaiDienUrl,
    string LoaiHoatDong,
    string MoTa,
    DateTimeOffset ThoiGianTao,
    string ThoiGianTuongDoi
);

public record DashboardOverviewResponse(
    ResidentCardDto TheCuDan,
    FeedbackCardDto ThePhanAnh,
    FinanceCardDto TheTaiChinh,
    VehicleCardDto ThePhuongTien,
    IReadOnlyList<RevenueByCategoryDto> DoanhThuTheoLoai,
    IReadOnlyList<UtilityBookingDto> DangKyTienIch,
    MaintenanceSummaryDto TongQuanBaoTri,
    IReadOnlyList<RecentActivityDto> HoatDongGanDay
);
