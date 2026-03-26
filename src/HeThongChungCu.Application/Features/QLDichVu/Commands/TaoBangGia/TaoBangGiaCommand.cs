using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.TaoBangGia;

public sealed record TaoBangGiaCommand(
    int DichVuId,
    string TenBangGia,
    DateTime NgayApDung,
    DateTime? NgayKetThuc,
    int LoaiDinhGiaId,
    decimal DonGia) : ICommand<BangGiaResponse>;

public sealed class TaoBangGiaCommandValidator : AbstractValidator<TaoBangGiaCommand>
{
    public TaoBangGiaCommandValidator()
    {
        RuleFor(x => x.DichVuId).NotEmpty();
        RuleFor(x => x.TenBangGia).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NgayApDung).NotEmpty();
        RuleFor(x => x.LoaiDinhGiaId).NotEmpty();
        RuleFor(x => x.DonGia).GreaterThanOrEqualTo(0);
        
        RuleFor(x => x)
            .Must(x => x.NgayKetThuc == null || x.NgayKetThuc > x.NgayApDung)
            .WithMessage("Ngày kết thúc phải lớn hơn ngày áp dụng.");
    }
}

internal sealed class TaoBangGiaCommandHandler : ICommandHandler<TaoBangGiaCommand, BangGiaResponse>
{
    private readonly IBangGiaEFRepository _bangGiaRepository;
    private readonly IDichVuEFRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaoBangGiaCommandHandler(
        IBangGiaEFRepository bangGiaRepository, 
        IDichVuEFRepository dichVuRepository, 
        IUnitOfWork unitOfWork)
    {
        _bangGiaRepository = bangGiaRepository;
        _dichVuRepository = dichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BangGiaResponse>> Handle(TaoBangGiaCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuRepository.GetByIdAsync(request.DichVuId, cancellationToken);
        if (dichVu is null)
        {
            return Result.Failure<BangGiaResponse>(new Error("DichVu.NotFound", "Không tìm thấy dịch vụ."));
        }

        // Check overlaps
        var existingPrices = await _bangGiaRepository.GetByDichVuIdAsync(request.DichVuId, cancellationToken);
        if (existingPrices.Any(p => p.IsOverlapping(request.NgayApDung, request.NgayKetThuc)))
        {
            return Result.Failure<BangGiaResponse>(new Error("BangGia.Overlap", "Thời gian áp dụng bảng giá bị chồng lấn với bảng giá hiện có."));
        }

        var loaiDinhGia = LoaiDinhGia.FromValue(request.LoaiDinhGiaId);
        if (loaiDinhGia is null)
        {
            return Result.Failure<BangGiaResponse>(new Error("LoaiDinhGia.Invalid", "Loại định giá không hợp lệ."));
        }

        var bangGia = new BangGia(
            request.DichVuId,
            request.TenBangGia,
            request.NgayApDung,
            loaiDinhGia,
            request.DonGia);

        if (request.NgayKetThuc.HasValue)
        {
            bangGia.UpdateInfo(bangGia.TenBangGia, bangGia.NgayApDung, request.NgayKetThuc, bangGia.DonGia, bangGia.LoaiDinhGiaId);
        }

        await _bangGiaRepository.AddAsync(bangGia, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BangGiaResponse(
            bangGia.Id,
            bangGia.DichVuId,
            bangGia.TenBangGia,
            bangGia.NgayApDung,
            bangGia.NgayKetThuc,
            bangGia.DonGia,
            bangGia.LoaiDinhGiaId.Value,
            bangGia.IsActive,
            new List<BangGiaLuyTienResponse>());
    }
}
