using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetDichVuById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.UpdateDichVu;

public class UpdateDichVuCommandHandler : ICommandHandler<UpdateDichVuCommand, DichVuDetailResponse>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IDichVuQueryRepository _dichVuQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDichVuCommandHandler(
        IDichVuCommandRepository dichVuCommandRepository,
        IDichVuQueryRepository dichVuQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _dichVuQueryRepository = dichVuQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DichVuDetailResponse>> Handle(UpdateDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuCommandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (dichVu == null)
            return DichVuErrors.NotFound;

        var loaiDichVu = LoaiDichVu.FromValue(request.LoaiDichVuId)!;
        dichVu.Update(
            request.TenDichVu,
            loaiDichVu,
            request.DonViTinh,
            request.MoTa,
            request.IconId,
            request.IsBatBuoc,
            request.SoLuongToiDa);

        _dichVuCommandRepository.Update(dichVu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _dichVuQueryRepository.GetByIdAsync(new GetDichVuByIdSpecification(dichVu.Id), cancellationToken);
        return response != null
            ? response
            : DichVuErrors.NotFound;
    }
}
