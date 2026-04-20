using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.UpdateDichVu;

public class UpdateDichVuCommandHandler : ICommandHandler<UpdateDichVuCommand, DichVuResponse>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDichVuCommandHandler(
        IDichVuCommandRepository dichVuCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DichVuResponse>> Handle(UpdateDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuCommandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (dichVu == null)
            return DichVuErrors.NotFound;

        dichVu.Update(
            dichVu.TenDichVu,
            dichVu.LoaiDichVuId,
            dichVu.DonViTinh,
            dichVu.MoTa,
            request.IconId,
            dichVu.IsBatBuoc,
            dichVu.SoLuongToiDa);

        _dichVuCommandRepository.Update(dichVu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DichVuResponse
        {
            Id = dichVu.Id,
            MaDichVu = dichVu.MaDichVu,
            TenDichVu = dichVu.TenDichVu,
            LoaiDichVuId = dichVu.LoaiDichVuId.Value,
            LoaiDichVuTen = dichVu.LoaiDichVuId.Name,
            DonViTinh = dichVu.DonViTinh,
            MoTa = dichVu.MoTa,
            IsBatBuoc = dichVu.IsBatBuoc,
            SoLuongToiDa = dichVu.SoLuongToiDa,
            TrangThaiDichVuId = dichVu.TrangThaiId.Value,
            TrangThaiDichVuTen = dichVu.TrangThaiId.Name
        });
    }
}
