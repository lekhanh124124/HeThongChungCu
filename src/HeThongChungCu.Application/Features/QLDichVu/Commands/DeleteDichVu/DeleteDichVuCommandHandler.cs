using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteDichVu;

public class DeleteDichVuCommandHandler : ICommandHandler<DeleteDichVuCommand, bool>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IDangKyDichVuCommandRepository _dangKyDichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDichVuCommandHandler(
        IDichVuCommandRepository dichVuCommandRepository, 
        IDangKyDichVuCommandRepository dangKyDichVuRepository,
        IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _dangKyDichVuRepository = dangKyDichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVus = await _dichVuCommandRepository.GetByIdsWithAllAsync(request.Ids, cancellationToken);
        var foundIds = dichVus.Select(x => x.Id).ToList();
        var missingIds = request.Ids.Except(foundIds).ToList();

        if (missingIds.Count != 0)
        {
            return DichVuErrors.NotFoundByIds(missingIds);
        }

        foreach (var dichVu in dichVus)
        {
            var hasRegistrations = await _dangKyDichVuRepository.AnyByDichVuIdAsync(dichVu.Id, cancellationToken);
            if (hasRegistrations)
            {
                return new Error("DichVu.HasRegistrations", "Không được xóa dịch vụ đã có người đăng ký sử dụng.");
            }

            foreach (var bg in dichVu.BangGias)
            {
                _dichVuCommandRepository.RemoveBangGia(bg);
            }

            foreach (var kg in dichVu.KhungGios)
            {
                _dichVuCommandRepository.RemoveKhungGio(kg);
            }

            _dichVuCommandRepository.Remove(dichVu);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
