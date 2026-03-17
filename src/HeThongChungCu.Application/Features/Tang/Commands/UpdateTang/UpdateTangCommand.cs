using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Commands.UpdateTang;

public record UpdateTangCommand(
    int Id,
    int ToaNhaId,
    string MaTang,
    string TenTang,
    int LoaiTangId) : ICommand<TangDetailResponse>;
