using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.UpdateDotThanhToan;

public class UpdateDotThanhToanCommandHandler : ICommandHandler<UpdateDotThanhToanCommand, DotThanhToanDetailResponse>
{
    private readonly IDotThanhToanCommandRepository _dotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDotThanhToanCommandHandler(
        IDotThanhToanCommandRepository dotRepository,
        IUnitOfWork unitOfWork)
    {
        _dotRepository = dotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DotThanhToanDetailResponse>> Handle(UpdateDotThanhToanCommand request, CancellationToken cancellationToken)
    {
        var dot = await _dotRepository.GetByIdAsync(request.Id, cancellationToken);
        if (dot == null)
            return DotThanhToanErrors.NotFound;

        var ky = new KyThanhToan(request.Thang, request.Nam);
        var isDuplicate = await _dotRepository.ExistsByKyThanhToanExcludeIdAsync(ky, request.Id, cancellationToken);
        if (isDuplicate)
            return DotThanhToanErrors.AlreadyExists;

        dot.Update(request.TenDot, ky, request.GhiChu);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DotThanhToanDetailResponse
        {
            Id = dot.Id,
            TenDot = dot.TenDot,
            Thang = dot.KyThanhToan.Thang,
            Nam = dot.KyThanhToan.Nam,
            TrangThaiDotThanhToanId = dot.TrangThaiDotThanhToanId.Value,
            TrangThaiDotThanhToanTen = dot.TrangThaiDotThanhToanId.Name,
            NgayPhatHanh = dot.NgayPhatHanh,
            GhiChu = dot.GhiChu
        });
    }
}
