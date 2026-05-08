using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.CreatePhanAnh;

public class CreatePhanAnhCommandHandler : ICommandHandler<CreatePhanAnhCommand, PhanAnhResponse>
{
    private readonly IYeuCauPhanAnhCommandRepository _phanAnhCommandRepository;
    private readonly IYeuCauPhanAnhQueryRepository _phanAnhQueryRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePhanAnhCommandHandler(
        IYeuCauPhanAnhCommandRepository phanAnhCommandRepository,
        IYeuCauPhanAnhQueryRepository phanAnhQueryRepository,
        ICanHoCommandRepository canHoRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _phanAnhCommandRepository = phanAnhCommandRepository;
        _phanAnhQueryRepository = phanAnhQueryRepository;
        _canHoRepository = canHoRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhanAnhResponse>> Handle(CreatePhanAnhCommand command, CancellationToken cancellationToken)
    {
        // 1. Validate Apartment existence
        var canHo = await _canHoRepository.GetByIdAsync(command.CanHoId, cancellationToken);
        if (canHo == null)
            return CanHoErrors.NotFoundById(command.CanHoId);

        // 2. Fetch options for file attachments
        var tepTaiLieus = command.DanhSachTepIds != null && command.DanhSachTepIds.Count != 0
            ? await _tepTaiLieuRepository.GetByIdsAsync(command.DanhSachTepIds, cancellationToken)
            : [];

        var tepPhanAnhs = tepTaiLieus.Select(f =>
            f is TepYeuCauPhanAnh tpa ? tpa : new TepYeuCauPhanAnh(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList();

        // 3. Parse Category Enum
        var loaiPhanAnh = LoaiPhanAnh.FromValue(command.LoaiPhanAnhId);
        if (loaiPhanAnh == null)
            return Result.Failure<PhanAnhResponse>(new Error("LoaiPhanAnh.Invalid", "Loại phản ánh không hợp lệ."));

        // 4. Create main Entity with attachments passed directly
        var creationResult = YeuCauPhanAnh.Create(
            command.CanHoId,
            command.TieuDe,
            command.NoiDung,
            loaiPhanAnh,
            tepPhanAnhs,
            command.IsSubmit);

        if (creationResult.IsFailure)
            return creationResult.Errors;

        var phanAnh = creationResult.Value;

        // 5. Persistence
        await _phanAnhCommandRepository.AddAsync(phanAnh, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Build response
        var response = await _phanAnhQueryRepository.GetByIdAsync(new GetPhanAnhByIdSpecification(phanAnh.Id), cancellationToken);

        return response != null
            ? Result.Success<PhanAnhResponse>(response)
            : Result.Failure<PhanAnhResponse>(PhanAnhErrors.NotFound);
    }
}
