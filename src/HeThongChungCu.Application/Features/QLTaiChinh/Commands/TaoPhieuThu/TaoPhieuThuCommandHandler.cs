using HeThongChungCu.Application.Common.Interfaces.Persistences;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Commands.TaoPhieuThu;

public class TaoPhieuThuCommandHandler : ICommandHandler<TaoPhieuThuCommand, int>
{
    private readonly IQuyThuChiCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TaoPhieuThuCommandHandler(IQuyThuChiCommandRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(TaoPhieuThuCommand request, CancellationToken cancellationToken)
    {
        // Generate a transaction code
        var transactionCode = $"THU-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

        var phuongThuc = PhuongThucThanhToan.FromValue(request.PhuongThucThanhToanId);

        var result = QuyThuChi.CreateThu(
            transactionCode,
            request.NgayGiaoDich,
            phuongThuc!,
            request.NguoiGiaoDich,
            request.ChungTuGoc);

        if (result.IsFailure)
        {
            return Result.Failure<int>(result.Errors);
        }

        var quyThu = result.Value;

        foreach (var chiTiet in request.ChiTiets)
        {
            quyThu.AddChiTiet(chiTiet.SoTien, chiTiet.NhomThongKe, chiTiet.GhiChu, chiTiet.DichVuId);
        }

        await _repository.AddAsync(quyThu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(quyThu.Id);
    }
}
