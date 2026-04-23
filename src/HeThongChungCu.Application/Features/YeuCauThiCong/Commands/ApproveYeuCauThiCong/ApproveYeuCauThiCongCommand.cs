using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.ApproveYeuCauThiCong;

public record ApproveYeuCauThiCongCommand(int Id) : ICommand<YeuCauThiCongResponse>;
