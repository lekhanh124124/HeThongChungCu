using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TiepNhanYeuCauSuaChua;

public record TiepNhanYeuCauSuaChuaCommand(int Id) : ICommand<YeuCauSuaChuaResponse>;
