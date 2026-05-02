using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ImportChiSo;

public record ImportChiSoCommand(
    Stream FileStream, 
    int Thang, 
    int Nam, 
    DateTimeOffset NgayGhiNhan) : ICommand<int>;
