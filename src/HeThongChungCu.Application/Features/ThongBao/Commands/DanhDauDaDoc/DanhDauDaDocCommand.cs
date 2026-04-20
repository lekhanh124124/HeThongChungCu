using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.ThongBao.Commands.DanhDauDaDoc;

public record DanhDauDaDocCommand(int PhanBoThongBaoId) : ICommand<bool>;
