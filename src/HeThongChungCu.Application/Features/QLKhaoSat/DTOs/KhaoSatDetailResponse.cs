using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

public class KhaoSatDetailResponse : KhaoSatResponse
{
    public List<CauHoiKhaoSatResponse> CauHois { get; set; } = [];
}
