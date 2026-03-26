using FluentValidation;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.HuyDangKyDichVu;

public sealed record HuyDangKyDichVuCommand(
    int Id,
    DateTime NgayKetThuc) : ICommand<DangKyDichVuResponse>;

public sealed class HuyDangKyDichVuCommandValidator : AbstractValidator<HuyDangKyDichVuCommand>
{
    public HuyDangKyDichVuCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NgayKetThuc).NotEmpty();
    }
}

internal sealed class HuyDangKyDichVuCommandHandler : ICommandHandler<HuyDangKyDichVuCommand, DangKyDichVuResponse>
{
    private readonly IDangKyDichVuEFRepository _dangKyDichVuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public HuyDangKyDichVuCommandHandler(IDangKyDichVuEFRepository dangKyDichVuRepository, IUnitOfWork unitOfWork)
    {
        _dangKyDichVuRepository = dangKyDichVuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DangKyDichVuResponse>> Handle(HuyDangKyDichVuCommand request, CancellationToken cancellationToken)
    {
        var registration = await _dangKyDichVuRepository.GetByIdAsync(request.Id, cancellationToken);
        if (registration is null)
        {
            return Result.Failure<DangKyDichVuResponse>(new Error("DangKyDichVu.NotFound", "Không tìm thấy thông tin đăng ký dịch vụ."));
        }

        try 
        {
            registration.HuyDangKy(request.NgayKetThuc);
        }
        catch (HeThongChungCu.Domain.Exceptions.BusinessException ex)
        {
            return Result.Failure<DangKyDichVuResponse>(new Error("DangKyDichVu.BusinessError", ex.Message));
        }

        _dangKyDichVuRepository.Update(registration);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DangKyDichVuResponse(
            registration.Id,
            registration.CanHoId,
            registration.DichVuId,
            registration.NgayBatDau,
            registration.NgayKetThuc,
            registration.IsActive);
    }
}
