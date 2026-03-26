using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.NgungDichVu;

public sealed record NgungDichVuCommand(int Id) : ICommand<DichVuResponse>;

internal sealed class NgungDichVuCommandHandler : ICommandHandler<NgungDichVuCommand, DichVuResponse>
{
    private readonly IDichVuEFRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NgungDichVuCommandHandler(IDichVuEFRepository dichVuRepository, IUnitOfWork unitOfWork)
    {
        _dichVuRepository = dichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DichVuResponse>> Handle(NgungDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuRepository.GetByIdAsync(request.Id, cancellationToken);
        if (dichVu is null)
        {
            return Result.Failure<DichVuResponse>(new Error("DichVu.NotFound", "Không tìm thấy dịch vụ."));
        }

        dichVu.Deactivate();

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
