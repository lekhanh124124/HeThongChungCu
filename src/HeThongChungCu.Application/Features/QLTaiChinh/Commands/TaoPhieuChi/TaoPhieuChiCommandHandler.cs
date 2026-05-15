using HeThongChungCu.Application.Common.Interfaces.Persistences;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Commands.TaoPhieuChi;

public class TaoPhieuChiCommandHandler : ICommandHandler<TaoPhieuChiCommand, int>
{
    private readonly IQuyThuChiCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TaoPhieuChiCommandHandler(IQuyThuChiCommandRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(TaoPhieuChiCommand request, CancellationToken cancellationToken)
    {
        // Generate a transaction code
        var transactionCode = $"CHI-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

        var phuongThuc = PhuongThucThanhToan.FromValue(request.PhuongThucThanhToanId);

        var result = QuyThuChi.CreateChi(
            transactionCode,
            request.NgayGiaoDich,
            phuongThuc!,
            request.NguoiGiaoDich,
            request.ChungTuGoc);

        if (result.IsFailure)
        {
            return Result.Failure<int>(result.Errors);
        }

        var quyChi = result.Value;

        foreach (var chiTiet in request.ChiTiets)
        {
            quyChi.AddChiTiet(chiTiet.SoTien, chiTiet.NhomThongKe, chiTiet.GhiChu);
        }

        await _repository.AddAsync(quyChi, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(quyChi.Id);
    }
}
