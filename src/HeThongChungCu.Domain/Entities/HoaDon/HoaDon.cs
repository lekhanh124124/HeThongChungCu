using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class HoaDon : AggregateRoot
{
    public string MaHoaDon { get; private set; } = string.Empty;
    public int CanHoId { get; private set; }
    public int Thang { get; private set; }
    public int Nam { get; private set; }
    public DateTime NgayTao { get; private set; }
    public DateTime HanThanhToan { get; private set; }
    public TrangThaiHoaDon TrangThaiHoaDonId { get; private set; } = null!;
    public string GhiChu { get; private set; } = string.Empty;

    private readonly List<ChiTietHoaDon> _chiTietHoaDons = [];
    public IReadOnlyCollection<ChiTietHoaDon> ChiTietHoaDons => _chiTietHoaDons.AsReadOnly();

    private readonly List<ThanhToan> _thanhToans = [];
    public IReadOnlyCollection<ThanhToan> ThanhToans => _thanhToans.AsReadOnly();

    private readonly List<LaiChamTra> _laiChamTras = [];
    public IReadOnlyCollection<LaiChamTra> LaiChamTras => _laiChamTras.AsReadOnly();

    private HoaDon() { } // EF Core

    public HoaDon(
        string maHoaDon,
        int canHoId,
        int thang,
        int nam,
        DateTime hanThanhToan,
        string ghiChu = "")
    {
        if (string.IsNullOrWhiteSpace(maHoaDon))
            throw new BusinessException("Mã hóa đơn không được để trống.");

        MaHoaDon = maHoaDon;
        CanHoId = canHoId;
        Thang = thang;
        Nam = nam;
        NgayTao = DateTime.Now;
        HanThanhToan = hanThanhToan;
        TrangThaiHoaDonId = TrangThaiHoaDon.ChuaThanhToan;
        GhiChu = ghiChu;
    }

    public void AddDetail(
        int dichVuId,
        string tenDichVu,
        double soLuong,
        decimal donGia,
        double? chiSoDau = null,
        double? chiSoCuoi = null)
    {
        var detail = new ChiTietHoaDon(Id, LoaiChiTietHoaDon.DichVu, dichVuId, tenDichVu, soLuong, donGia, chiSoDau, chiSoCuoi);
        _chiTietHoaDons.Add(detail);
    }

    public void AddPreviousDebt(decimal amount, string period)
    {
        if (amount <= 0) return;
        
        var detail = new ChiTietHoaDon(Id, LoaiChiTietHoaDon.NoCu, 0, $"Nợ cũ kỳ {period}", 1, amount, ghiChu: "Nợ tồn từ kỳ trước");
        _chiTietHoaDons.Add(detail);
    }

    public void ApplyLateInterest(CauHinhLai cauHinh, DateTime ngayHienTai)
    {
        if (TrangThaiHoaDonId == TrangThaiHoaDon.DaThanhToan) return;
        
        // 1. Chống double apply trong cùng 1 ngày
        if (_laiChamTras.Any(x => x.NgayTinh.Date == ngayHienTai.Date))
            return;

        // 2. Tính tiền gốc (Principal) - Chỉ lấy loại Dịch vụ (Không tính trên nợ cũ hay lãi cũ)
        var tongTienGoc = _chiTietHoaDons
            .Where(x => x.LoaiChiTietId == LoaiChiTietHoaDon.DichVu)
            .Sum(x => x.ThanhTien);

        // Trừ đi số tiền đã thanh toán (ưu tiên trừ vào gốc trước)
        var tongDaThanhToan = CalculatePaidTotal();
        var tienGocChuaThanhToan = Math.Max(0, tongTienGoc - tongDaThanhToan);

        if (tienGocChuaThanhToan <= 0) return;

        var hanThanhToanGiaHan = HanThanhToan.AddDays(cauHinh.SoNgayChoPhep);
        if (ngayHienTai <= hanThanhToanGiaHan) return;

        var tongSoNgayQuaHan = (ngayHienTai - HanThanhToan).Days;
        
        // 3. Tính lãi incremental
        var lanTinhLaiCuoi = _laiChamTras.OrderByDescending(x => x.NgayTinh).FirstOrDefault();
        var soNgayDaTinh = lanTinhLaiCuoi?.SoNgayCham ?? 0;
        var soNgayQuaHanMoi = tongSoNgayQuaHan - soNgayDaTinh;

        if (soNgayQuaHanMoi <= 0) return;

        var soTienLaiPhat = tienGocChuaThanhToan * (cauHinh.LaiSuatThang / 100 / 30) * soNgayQuaHanMoi;

        if (soTienLaiPhat > 0)
        {
            var laiChamTra = new LaiChamTra(Id, ngayHienTai, tienGocChuaThanhToan, tongSoNgayQuaHan, cauHinh.LaiSuatThang, soTienLaiPhat);
            _laiChamTras.Add(laiChamTra);

            var detail = new ChiTietHoaDon(Id, LoaiChiTietHoaDon.LaiChamTra, 0, "Lãi chậm trả", 1, soTienLaiPhat, ghiChu: $"Tính lãi cho {soNgayQuaHanMoi} ngày quá hạn mới (Tổng nợ gốc: {tienGocChuaThanhToan:N0})");
            _chiTietHoaDons.Add(detail);
            
            UpdateOverdueStatus(ngayHienTai, cauHinh.NguongQuaHanNhe, cauHinh.NguongQuaHanNang);
        }
    }

    public void UpdateOverdueStatus(DateTime ngayHienTai, int nguongNhe, int nguongNang)
    {
        if (TrangThaiHoaDonId == TrangThaiHoaDon.DaThanhToan || TrangThaiHoaDonId == TrangThaiHoaDon.DaHuy)
            return;

        if (ngayHienTai <= HanThanhToan) return;

        var soNgayQuaHan = (ngayHienTai - HanThanhToan).Days;

        if (soNgayQuaHan >= nguongNang)
            TrangThaiHoaDonId = TrangThaiHoaDon.QuaHanNang;
        else if (soNgayQuaHan >= nguongNhe)
            TrangThaiHoaDonId = TrangThaiHoaDon.QuaHanNhe;
        else
            TrangThaiHoaDonId = TrangThaiHoaDon.QuaHan;
    }

    public decimal CalculateTotalBalance() => _chiTietHoaDons.Sum(x => x.ThanhTien);
    public decimal CalculatePaidTotal() => _thanhToans.Sum(x => x.SoTien);

    public void AddThanhToan(
        decimal soTien,
        DateTime ngayThanhToan,
        PhuongThucThanhToan phuongThucId,
        string maGiaoDich = "",
        string noiDung = "")
    {
        if (soTien <= 0)
            throw new BusinessException("Số tiền thanh toán phải lớn hơn 0.");

        if (_thanhToans.Any(x => x.MaGiaoDich == maGiaoDich))
            throw new BusinessException("Giao dịch đã tồn tại.");

        var thanhToan = new ThanhToan(Id, ngayThanhToan, soTien, phuongThucId, maGiaoDich, noiDung);
        _thanhToans.Add(thanhToan);

        var tongTien = CalculateTotalBalance();
        var tongDaThanhToan = CalculatePaidTotal();

        if (tongDaThanhToan >= tongTien && tongTien > 0)
        {
            TrangThaiHoaDonId = TrangThaiHoaDon.DaThanhToan;
        }
        else if (tongDaThanhToan > 0)
        {
            TrangThaiHoaDonId = TrangThaiHoaDon.ThanhToanMotPhan;
        }
    }

    public void UpdateStatus(TrangThaiHoaDon nextStatus)
    {
        TrangThaiHoaDonId = nextStatus;
    }
}
