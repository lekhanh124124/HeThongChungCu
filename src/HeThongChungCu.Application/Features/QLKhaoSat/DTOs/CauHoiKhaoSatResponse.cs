using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

public class CauHoiKhaoSatResponse
{
    public int Id { get; set; }
    public string NoiDungCauHoi { get; set; } = string.Empty;
    public bool IsBatBuoc { get; set; }
    public bool IsMultiSelect { get; set; }
    public List<LuaChonKhaoSatResponse> LuaChons { get; set; } = [];
}
