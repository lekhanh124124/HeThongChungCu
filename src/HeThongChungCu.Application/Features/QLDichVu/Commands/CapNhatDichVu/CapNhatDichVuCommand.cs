using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CapNhatDichVu;

public sealed record CapNhatDichVuCommand(
    int Id,
    string TenDichVu,
    string DonViTinh) : ICommand<bool>;

public sealed class CapNhatDichVuCommandValidator : AbstractValidator<CapNhatDichVuCommand>
{
    public CapNhatDichVuCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TenDichVu).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DonViTinh).NotEmpty().MaximumLength(50);
    }
}

internal sealed class CapNhatDichVuCommandHandler : ICommandHandler<CapNhatDichVuCommand, bool>
{
    private readonly IDichVuEFRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatDichVuCommandHandler(IDichVuEFRepository dichVuRepository, IUnitOfWork unitOfWork)
    {
        _dichVuRepository = dichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CapNhatDichVuCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _dichVuRepository.GetByIdAsync(request.Id, cancellationToken);
        if (dichVu is null)
        {
            return Result.Failure<bool>(new Error("DichVu.NotFound", "Không tìm thấy dịch vụ."));
        }

        dichVu.Update(request.TenDichVu, request.DonViTinh);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
