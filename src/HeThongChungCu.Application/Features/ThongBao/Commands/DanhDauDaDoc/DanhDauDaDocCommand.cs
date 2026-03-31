using MediatR;

namespace HeThongChungCu.Application.Features.ThongBao.Commands.DanhDauDaDoc;

public record DanhDauDaDocCommand(int PhanBoThongBaoId) : IRequest<Result<bool>>;
