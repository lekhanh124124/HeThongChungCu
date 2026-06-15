using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using System.Linq;

namespace HeThongChungCu.Application.Features.QLNhanVien.Commands.DeleteNhanVien;

public class DeleteNhanVienCommandHandler : ICommandHandler<DeleteNhanVienCommand, IReadOnlyList<int>>
{
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNhanVienCommandHandler(
        INhanVienCommandRepository nhanVienRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _nhanVienRepository = nhanVienRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<int>>> Handle(DeleteNhanVienCommand request, CancellationToken cancellationToken)
    {
        var nhanViens = (await _nhanVienRepository.GetByIdsAsync(request.Ids, cancellationToken)).ToList();

        var notFoundIds = request.Ids.Except(nhanViens.Select(x => x.Id)).ToList();
        if (notFoundIds.Count != 0)
            return NhanVienErrors.NotFoundByIds(notFoundIds);

        foreach (var nhanVien in nhanViens)
        {
            var isAssigned = await _nhanVienRepository.HasAssignedRequestsAsync(nhanVien.Id, cancellationToken);
            if (isAssigned)
            {
                return new Error("NhanVien.HasAssignedRequests", "Không được xóa nhân viên đã được phân công xử lý yêu cầu.");
            }

            nhanVien.CapNhatTrangThai(TrangThaiNhanVien.DaNghiViec, _dateTimeProvider.Now.DateTime);
            await _nhanVienRepository.UpdateAsync(nhanVien, cancellationToken);
            await _nhanVienRepository.DeleteAsync(nhanVien, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<IReadOnlyList<int>>(nhanViens.Select(x => x.Id).ToList());
    }
}
