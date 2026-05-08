using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

public class KetQuaKhaoSatResponse
{
    public int KhaoSatId { get; set; }
    public string TieuDeKhaoSat { get; set; } = string.Empty;
    public int TongSoCanHo { get; set; }
    public int SoCanHoDaThamGia { get; set; }
    public decimal TyLeThamGia { get; set; } // % tham gia thực tế
    public decimal TyleThamGiaToiThieu { get; set; } // % tối thiểu để có hiệu lực pháp lý
    public bool IsHieuLuc { get; set; } // true nếu TyLeThamGia >= TyleThamGiaToiThieu
    public int CoCheTinhDiemId { get; set; }
    public string CoCheTinhDiemTen { get; set; } = string.Empty;
    public List<KetQuaCauHoiResponse> KetQuaCauHois { get; set; } = [];
}

public class KetQuaCauHoiResponse
{
    public int CauHoiId { get; set; }
    public string NoiDungCauHoi { get; set; } = string.Empty;
    public bool IsMultiSelect { get; set; }
    public List<KetQuaLuaChonResponse> KetQuaLuaChons { get; set; } = [];
}

public class KetQuaLuaChonResponse
{
    public int LuaChonId { get; set; }
    public string NoiDungLuaChon { get; set; } = string.Empty;
    public bool IsUngVienBQT { get; set; }
    
    // SoLuongPhieuBau sử dụng decimal vì nếu theo cơ chế m2, số lượng phiếu bầu chính là tổng diện tích m2 của các căn hộ chọn phương án này!
    public decimal SoLuongPhieuBau { get; set; } 
    public decimal TyLePhanTram { get; set; } // % so với tổng số lượng phiếu/m2 đã bầu
}
