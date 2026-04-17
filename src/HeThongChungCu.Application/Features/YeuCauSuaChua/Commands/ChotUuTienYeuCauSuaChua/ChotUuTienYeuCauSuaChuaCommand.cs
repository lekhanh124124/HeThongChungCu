using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.ChotUuTienYeuCauSuaChua;

public record ChotUuTienYeuCauSuaChuaCommand(int Id, int MucDoUuTienChotId) : ICommand<YeuCauSuaChuaResponse>;
