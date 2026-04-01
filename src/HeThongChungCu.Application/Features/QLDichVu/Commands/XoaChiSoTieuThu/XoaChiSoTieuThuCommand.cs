using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.XoaChiSoTieuThu;

public sealed record XoaChiSoTieuThuCommand(int Id) : ICommand<bool>;

public sealed class XoaChiSoTieuThuCommandValidator : AbstractValidator<XoaChiSoTieuThuCommand>
{
    public XoaChiSoTieuThuCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
    }
}

internal sealed class XoaChiSoTieuThuCommandHandler : ICommandHandler<XoaChiSoTieuThuCommand, bool>
{
    private readonly IChiSoTieuThuCommandRepository _chiSoTieuThuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public XoaChiSoTieuThuCommandHandler(IChiSoTieuThuCommandRepository chiSoTieuThuRepository, IUnitOfWork unitOfWork)
    {
        _chiSoTieuThuRepository = chiSoTieuThuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(XoaChiSoTieuThuCommand request, CancellationToken cancellationToken)
    {
        var chiSoTieuThu = await _chiSoTieuThuRepository.GetByIdAsync(request.Id, cancellationToken);
        if (chiSoTieuThu is null)
        {
            return Result.Failure<bool>(ChiSoTieuThuErrors.NotFound);
        }

        if (chiSoTieuThu.IsLock)
        {
            return Result.Failure<bool>(ChiSoTieuThuErrors.Locked);
        }

        // Resolve ambiguous Remove call by being explicit about the interface
        _chiSoTieuThuRepository.Remove(chiSoTieuThu); 
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
