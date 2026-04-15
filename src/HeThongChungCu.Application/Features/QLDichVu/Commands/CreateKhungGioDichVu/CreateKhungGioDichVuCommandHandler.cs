using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateKhungGioDichVu;

public class CreateKhungGioDichVuCommandHandler : ICommandHandler<CreateKhungGioDichVuCommand, KhungGioDichVuResponse>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateKhungGioDichVuCommandHandler(
        IDichVuCommandRepository dichVuCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<KhungGioDichVuResponse>> Handle(CreateKhungGioDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuCommandRepository.GetByIdWithKhungGiosAsync(request.DichVuId, cancellationToken);
        if (dichVu == null)
            return Result.Failure<KhungGioDichVuResponse>(DichVuErrors.NotFoundById(request.DichVuId));

        var addResult = dichVu.AddKhungGio(
            request.GioBatDau,
            request.GioKetThuc,
            request.TenKhungGio,
            request.NgayTrongTuan.HasValue ? NgayTrongTuan.FromValue(request.NgayTrongTuan.Value) : null);

        if (addResult.IsFailure)
        {
            return Result.Failure<KhungGioDichVuResponse>(addResult.Errors);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map the newly created KhungGio from the result directly to response
        var newKhungGio = addResult.Value;

        var response = new KhungGioDichVuResponse
        {
            Id = newKhungGio.Id,
            DichVuId = newKhungGio.DichVuId,
            GioBatDau = newKhungGio.GioBatDau,
            GioKetThuc = newKhungGio.GioKetThuc,
            TenKhungGio = newKhungGio.TenKhungGio,
            NgayTrongTuan = newKhungGio.NgayTrongTuan?.Value
        };

        return Result.Success(response);
    }
}
