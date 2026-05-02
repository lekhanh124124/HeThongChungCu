using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.CreateDotThanhToan;

public class CreateDotThanhToanCommandHandler : ICommandHandler<CreateDotThanhToanCommand, DotThanhToanDetailResponse>
{
    private readonly IDotThanhToanCommandRepository _dotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDotThanhToanCommandHandler(
        IDotThanhToanCommandRepository dotRepository,
        IUnitOfWork unitOfWork)
    {
        _dotRepository = dotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DotThanhToanDetailResponse>> Handle(CreateDotThanhToanCommand request, CancellationToken cancellationToken)
    {
        var ky = new KyThanhToan(request.Thang, request.Nam);

        var dot = await _dotRepository.GetLatestOpenByKyAsync(ky, cancellationToken);
        if (dot != null)
            return DotThanhToanErrors.AlreadyExists;

        string tenDot = $"Đợt thanh toán {request.Thang}/{request.Nam}";

        var dotResult = DotThanhToan.Create(tenDot, ky);
        if (dotResult.IsFailure)
            return dotResult.Errors;

        dot = dotResult.Value;
        await _dotRepository.AddAsync(dot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DotThanhToanDetailResponse
        {
            Id = dot.Id,
            TenDot = dot.TenDot,
            Thang = dot.KyThanhToan.Thang,
            Nam = dot.KyThanhToan.Nam,
            TrangThaiDotThanhToanId = dot.TrangThaiDotThanhToanId.Value,
            TrangThaiDotThanhToanTen = dot.TrangThaiDotThanhToanId.Name,
            NgayPhatHanh = dot.NgayPhatHanh,
            GhiChu = dot.GhiChu
        }; ;
    }
}