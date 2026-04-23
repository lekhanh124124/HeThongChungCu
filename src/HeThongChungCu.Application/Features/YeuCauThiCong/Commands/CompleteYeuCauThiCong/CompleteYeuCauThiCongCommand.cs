using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.CompleteYeuCauThiCong;

public record CompleteYeuCauThiCongCommand(int Id) : ICommand<YeuCauThiCongResponse>;
