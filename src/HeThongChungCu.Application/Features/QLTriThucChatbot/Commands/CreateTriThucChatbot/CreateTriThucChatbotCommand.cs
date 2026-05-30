using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.CreateTriThucChatbot;

public record CreateTriThucChatbotCommand(
    string TieuDe,
    string NoiDung,
    string DanhMuc,
    int ThuTuHienThi = 0) : ICommand<TriThucChatbotResponse>;
