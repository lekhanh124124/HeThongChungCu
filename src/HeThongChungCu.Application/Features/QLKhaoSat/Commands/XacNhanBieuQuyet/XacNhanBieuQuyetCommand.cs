using System.Collections.Generic;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.XacNhanBieuQuyet;

public record XacNhanBieuQuyetCommand : ICommand<bool>
{
    public int KhaoSatId { get; init; }
    public int CanHoId { get; init; }
    public string OtpCode { get; init; } = string.Empty;
    public List<ChiTietLuaChonDto> TraLois { get; init; } = [];
}

public class ChiTietLuaChonDto
{
    public int LuaChonId { get; set; }
    public string? NoiDungTraLoiTuDo { get; set; }
}
