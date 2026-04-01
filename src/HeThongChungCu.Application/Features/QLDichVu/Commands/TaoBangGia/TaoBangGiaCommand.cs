using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

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
        RuleFor(x => x.DichVuId).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
        RuleFor(x => x.TenBangGia)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(200).WithMessage(ValidationErrors.MaxLength(200).Description);
        RuleFor(x => x.NgayApDung).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
        RuleFor(x => x.LoaiDinhGiaId).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
        RuleFor(x => x.DonGia).GreaterThanOrEqualTo(0).WithMessage(ValidationErrors.Range(0, double.MaxValue).Description);
        
        RuleFor(x => x)
            .Must(x => x.NgayKetThuc == null || x.NgayKetThuc > x.NgayApDung)
            .WithMessage(ValidationErrors.InvalidDateRange.Description);
    }
}

internal sealed class TaoBangGiaCommandHandler : ICommandHandler<TaoBangGiaCommand, BangGiaResponse>
{
    private readonly IBangGiaCommandRepository _bangGiaRepository;
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaoBangGiaCommandHandler(
        IBangGiaCommandRepository bangGiaRepository, 
        IDichVuCommandRepository dichVuRepository, 
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
            return Result.Failure<BangGiaResponse>(DichVuErrors.NotFound);
        }

        // Check overlaps
        var existingPrices = await _bangGiaRepository.GetByDichVuIdAsync(request.DichVuId, cancellationToken);
        if (existingPrices.Any(p => p.IsOverlapping(request.NgayApDung, request.NgayKetThuc)))
        {
            return Result.Failure<BangGiaResponse>(BangGiaErrors.Overlap);
        }

        var loaiDinhGia = LoaiDinhGia.FromValue(request.LoaiDinhGiaId);
        if (loaiDinhGia is null)
        {
            return Result.Failure<BangGiaResponse>(BangGiaErrors.InvalidType(LoaiDinhGia.GetAll().Select(l => $"{l.Value} ({l.Name})")));
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
