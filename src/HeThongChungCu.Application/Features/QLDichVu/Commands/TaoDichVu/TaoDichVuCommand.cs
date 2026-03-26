using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.TaoDichVu;

public sealed record TaoDichVuCommand(
    string MaDichVu,
    string TenDichVu,
    string DonViTinh) : ICommand<DichVuResponse>;

public sealed class TaoDichVuCommandValidator : AbstractValidator<TaoDichVuCommand>
{
    public TaoDichVuCommandValidator()
    {
        RuleFor(x => x.MaDichVu).NotEmpty().MaximumLength(20);
        RuleFor(x => x.TenDichVu).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DonViTinh).NotEmpty().MaximumLength(50);
    }
}

internal sealed class TaoDichVuCommandHandler : ICommandHandler<TaoDichVuCommand, DichVuResponse>
{
    private readonly IDichVuEFRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaoDichVuCommandHandler(IDichVuEFRepository dichVuRepository, IUnitOfWork unitOfWork)
    {
        _dichVuRepository = dichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DichVuResponse>> Handle(TaoDichVuCommand request, CancellationToken cancellationToken)
    {
        var isCodeUnique = await _dichVuRepository.MaDichVuExistsAsync(request.MaDichVu, cancellationToken);
        if (isCodeUnique)
        {
            return Result.Failure<DichVuResponse>(new Error("DichVu.MaDichVuAlreadyExists", "Mã dịch vụ đã tồn tại"));
        }

        var dichVu = new DichVu(
            request.MaDichVu,
            request.TenDichVu,
            request.DonViTinh);

        await _dichVuRepository.AddAsync(dichVu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DichVuResponse(dichVu.Id, dichVu.MaDichVu, dichVu.TenDichVu, dichVu.DonViTinh, dichVu.IsActive);
    }
}
