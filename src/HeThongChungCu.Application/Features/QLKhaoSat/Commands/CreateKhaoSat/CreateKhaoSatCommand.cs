using System;
using System.Collections.Generic;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.CreateKhaoSat;

public record CreateKhaoSatCommand : ICommand<KhaoSatResponse>
{
    public string TieuDe { get; init; } = string.Empty;
    public string MoTa { get; init; } = string.Empty;
    public int LoaiKhaoSatId { get; init; }
    public int CoCheTinhDiemId { get; init; }
    public DateTimeOffset NgayBatDau { get; init; }
    public DateTimeOffset NgayKetThuc { get; init; }
    public decimal TyleThamGiaToiThieu { get; init; }
    public decimal TyLeDongYToiThieu { get; init; }
    public bool IsAnDanh { get; init; }
    public List<CreateCauHoiDto> CauHois { get; init; } = [];
}

public class CreateCauHoiDto
{
    public string NoiDungCauHoi { get; set; } = string.Empty;
    public bool IsBatBuoc { get; set; }
    public bool IsMultiSelect { get; set; }
    public List<CreateLuaChonDto> LuaChons { get; set; } = [];
}

public class CreateLuaChonDto
{
    public string NoiDungLuaChon { get; set; } = string.Empty;
    public bool IsUngVienBQT { get; set; }
    public string? TieuSuUngVien { get; set; }
    public int? UngVienId { get; set; }
}
