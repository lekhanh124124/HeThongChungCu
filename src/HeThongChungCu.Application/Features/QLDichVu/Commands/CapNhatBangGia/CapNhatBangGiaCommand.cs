using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CapNhatBangGia;

public sealed record CapNhatBangGiaCommand(
    int Id,
    string TenBangGia,
    DateTime NgayApDung,
    DateTime? NgayKetThuc,
    decimal DonGia) : ICommand<BangGiaResponse>;

public sealed class CapNhatBangGiaCommandValidator : AbstractValidator<CapNhatBangGiaCommand>
{
    public CapNhatBangGiaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
        RuleFor(x => x.TenBangGia)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(200).WithMessage(ValidationErrors.MaxLength(200).Description);
        RuleFor(x => x.NgayApDung).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
        RuleFor(x => x.DonGia).GreaterThanOrEqualTo(0).WithMessage(ValidationErrors.Range(0, double.MaxValue).Description);
        
        RuleFor(x => x)
            .Must(x => x.NgayKetThuc == null || x.NgayKetThuc > x.NgayApDung)
            .WithMessage(ValidationErrors.InvalidDateRange.Description);
    }
}

internal sealed class CapNhatBangGiaCommandHandler : ICommandHandler<CapNhatBangGiaCommand, BangGiaResponse>
{
    private readonly IBangGiaCommandRepository _bangGiaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatBangGiaCommandHandler(IBangGiaCommandRepository bangGiaRepository, IUnitOfWork unitOfWork)
    {
        _bangGiaRepository = bangGiaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BangGiaResponse>> Handle(CapNhatBangGiaCommand request, CancellationToken cancellationToken)
    {
        var bangGia = await _bangGiaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (bangGia is null)
        {
            return Result.Failure<BangGiaResponse>(BangGiaErrors.NotFound);
        }

        // Check overlaps excluding current
        var existingPrices = await _bangGiaRepository.GetByDichVuIdAsync(bangGia.DichVuId, cancellationToken);
        if (existingPrices.Any(p => p.Id != request.Id && p.IsOverlapping(request.NgayApDung, request.NgayKetThuc)))
        {
            return Result.Failure<BangGiaResponse>(BangGiaErrors.Overlap);
        }

        bangGia.UpdateInfo(
            request.TenBangGia,
            request.NgayApDung,
            request.NgayKetThuc,
            request.DonGia,
            bangGia.LoaiDinhGiaId);

        _bangGiaRepository.Update(bangGia);
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
            bangGia.BangGiaLuyTiens.Select(l => new BangGiaLuyTienResponse(l.TuMuc, l.DenMuc, l.DonGia)).ToList());
    }
}
