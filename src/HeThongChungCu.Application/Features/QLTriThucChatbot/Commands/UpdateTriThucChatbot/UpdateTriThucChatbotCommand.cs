using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.UpdateTriThucChatbot;

public record UpdateTriThucChatbotCommand(
    int Id,
    string TieuDe,
    string NoiDung,
    string DanhMuc,
    int ThuTuHienThi) : ICommand<TriThucChatbotResponse>;
