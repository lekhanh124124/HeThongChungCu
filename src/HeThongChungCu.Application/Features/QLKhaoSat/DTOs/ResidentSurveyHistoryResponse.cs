using System;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

public class ResidentSurveyHistoryResponse
{
    public int KhaoSatId { get; set; }
    public string TieuDeKhaoSat { get; set; } = null!;
    public DateTimeOffset NgayThamGia { get; set; }
    public List<QuestionAnswerResponse> ChiTietLuaChon { get; set; } = [];
}

public class QuestionAnswerResponse
{
    public string CauHoi { get; set; } = null!;
    public List<OptionSelectionResponse> LuaChons { get; set; } = [];
    public string? NoiDungTuDo { get; set; }
}

public class OptionSelectionResponse
{
    public string NoiDungLuaChon { get; set; } = null!;
    public bool IsSelected { get; set; }
}
