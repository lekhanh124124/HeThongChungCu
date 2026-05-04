using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.DeleteChiSoTieuThu;

public record DeleteChiSoTieuThuCommand(List<int> Ids) : ICommand<int>;
