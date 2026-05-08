using System;
using System.Collections.Generic;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

public class PhanAnhDetailResponse : PhanAnhResponse
{
    public string NoiDung { get; set; } = string.Empty;
    public int? DiemDanhGia { get; set; }
    public string? NhanXetDanhGia { get; set; }
    public DateTimeOffset? NgayDanhGia { get; set; }
    
    // Audit logs / chat replies
    public List<TraLoiPhanAnhResponse> TraLoiPhanAnhs { get; set; } = [];
    
    // Attachments
    public List<TepTaiLieuResponse> DanhSachTep { get; set; } = [];
}
