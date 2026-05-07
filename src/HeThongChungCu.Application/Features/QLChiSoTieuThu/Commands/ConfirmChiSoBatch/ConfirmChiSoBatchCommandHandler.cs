using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ConfirmChiSoBatch;

public class ConfirmChiSoBatchCommandHandler : ICommandHandler<ConfirmChiSoBatchCommand, int>
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmChiSoBatchCommandHandler(
        IChiSoTieuThuCommandRepository chiSoRepository,
        IDichVuCommandRepository dichVuRepository,
        IUnitOfWork unitOfWork)
    {
        _chiSoRepository = chiSoRepository;
        _dichVuRepository = dichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(ConfirmChiSoBatchCommand request, CancellationToken cancellationToken)
    {
        // Validate dịch vụ nếu có truyền DichVuId
        if (request.DichVuId.HasValue)
        {
            var validationResult = await ValidateDichVuAsync(request.DichVuId.Value, cancellationToken);
            if (validationResult.IsFailure)
                return Result.Failure<int>(validationResult.Errors.First());
        }

        var draftChiSos = await _chiSoRepository.GetDraftByPeriodAsync(
            request.Thang, request.Nam, request.DichVuId, cancellationToken);

        if (draftChiSos.Count == 0)
        {
            return Result.Failure<int>(new Error(
                "Confirm.NoDraft",
                "Không tìm thấy chỉ số nào ở trạng thái Nháp cho kỳ này."));
        }

        var count = 0;
        foreach (var chiSo in draftChiSos)
        {
            var result = chiSo.Confirm();
            if (result.IsSuccess)
            {
                _chiSoRepository.Update(chiSo);
                count++;
            }
        }

        if (count > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(count);
    }

    private async Task<Result> ValidateDichVuAsync(int dichVuId, CancellationToken cancellationToken)
    {
        var service = await _dichVuRepository.GetByIdWithBangGiasAsync(dichVuId, cancellationToken);

        if (service == null)
            return Result.Failure(DichVuErrors.NotFoundById(dichVuId));

        if (service.TrangThaiId != TrangThaiDichVu.HoatDong && 
            service.TrangThaiId != TrangThaiDichVu.CanhBao)
        {
            return Result.Failure(DichVuErrors.NotActive(service.TenDichVu));
        }

        // Kiểm tra xem có bảng giá lũy tiến định kỳ nào đang active không
        var activePrice = service.GetCurrentPrice(DateTimeOffset.Now);

        if (activePrice == null || !activePrice.IsDinhKy || activePrice.LoaiDinhGiaId != LoaiDinhGia.LuyTien)
        {
            return Result.Failure(new Error("Confirm.InvalidServiceType",
                $"Dịch vụ '{service.TenDichVu}' không phải là dịch vụ tiêu thụ (không có bảng giá lũy tiến định kỳ đang áp dụng)."));
        }

        return Result.Success();
    }
}
