using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.XoaChiSoTieuThu;

public sealed record XoaChiSoTieuThuCommand(int Id) : ICommand<bool>;

public sealed class XoaChiSoTieuThuCommandValidator : AbstractValidator<XoaChiSoTieuThuCommand>
{
    public XoaChiSoTieuThuCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
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
            return Result.Failure<bool>(new Error("ChiSoTieuThu.NotFound", "Không tìm thấy chỉ số tiêu thụ."));
        }

        if (chiSoTieuThu.IsLock)
        {
            return Result.Failure<bool>(new Error("ChiSoTieuThu.Locked", "Không thể xóa chỉ số tiêu thụ đã bị khóa."));
        }

        // Resolve ambiguous Remove call by being explicit about the interface
        _chiSoTieuThuRepository.Remove(chiSoTieuThu); 
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
