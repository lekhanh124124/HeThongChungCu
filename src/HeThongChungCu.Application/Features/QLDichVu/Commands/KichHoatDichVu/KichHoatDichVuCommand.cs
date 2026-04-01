using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.KichHoatDichVu;

public sealed record KichHoatDichVuCommand(int Id) : ICommand<DichVuResponse>;

internal sealed class KichHoatDichVuCommandHandler : ICommandHandler<KichHoatDichVuCommand, DichVuResponse>
{
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public KichHoatDichVuCommandHandler(IDichVuCommandRepository dichVuRepository, IUnitOfWork unitOfWork)
    {
        _dichVuRepository = dichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DichVuResponse>> Handle(KichHoatDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuRepository.GetByIdAsync(request.Id, cancellationToken);
        if (dichVu is null)
        {
            return Result.Failure<DichVuResponse>(new Error("DichVu.NotFound", "Không tìm thấy dịch vụ."));
        }

        dichVu.Activate();

        _dichVuRepository.Update(dichVu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DichVuResponse(
            dichVu.Id,
            dichVu.MaDichVu,
            dichVu.TenDichVu,
            dichVu.DonViTinh,
            dichVu.IsActive);
    }
}
