using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Commands.UpdateTang;

public record UpdateTangCommand(
    int Id,
    string MaTang,
    string TenTang,
    int LoaiTangId,
    int ToaNhaId) : ICommand<TangDetailResponse>;
