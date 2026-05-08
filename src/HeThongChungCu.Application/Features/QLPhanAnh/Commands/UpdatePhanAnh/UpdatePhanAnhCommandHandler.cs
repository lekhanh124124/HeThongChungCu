using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.UpdatePhanAnh;

public class UpdatePhanAnhCommandHandler : ICommandHandler<UpdatePhanAnhCommand, PhanAnhResponse>
{
    private readonly IYeuCauPhanAnhCommandRepository _phanAnhCommandRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IYeuCauPhanAnhQueryRepository _phanAnhQueryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePhanAnhCommandHandler(
        IYeuCauPhanAnhCommandRepository phanAnhCommandRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IYeuCauPhanAnhQueryRepository phanAnhQueryRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _phanAnhCommandRepository = phanAnhCommandRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _phanAnhQueryRepository = phanAnhQueryRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhanAnhResponse>> Handle(UpdatePhanAnhCommand request, CancellationToken cancellationToken)
    {
        // 1. Authenticate user
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<PhanAnhResponse>(UserErrors.NotFound);

        // 2. Fetch feedback
        var phanAnh = await _phanAnhCommandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (phanAnh == null)
            return Result.Failure<PhanAnhResponse>(PhanAnhErrors.NotFoundById(request.Id));

        // 3. Guard ownership: only creator can edit/withdraw
        if (phanAnh.CreatedBy != userId)
            return Result.Failure<PhanAnhResponse>(PhanAnhErrors.Forbidden);

        if (request.IsWithdraw)
        {
            // Withdraw submitted/draft/returned feedback
            var withdrawResult = phanAnh.Withdraw();
            if (withdrawResult.IsFailure)
                return Result.Failure<PhanAnhResponse>(withdrawResult.Errors);
        }
        else
        {
            // Fetch new attachments if provided
            List<TepYeuCauPhanAnh>? tepPhanAnhs = null;
            if (request.DanhSachTepIds != null)
            {
                var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(request.DanhSachTepIds, cancellationToken);
                tepPhanAnhs = tepTaiLieus.Select(f =>
                    f is TepYeuCauPhanAnh tpa ? tpa : new TepYeuCauPhanAnh(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList();
            }

            // Resolve Category Enum
            LoaiPhanAnh? loaiPhanAnh = null;
            if (request.LoaiPhanAnhId.HasValue)
            {
                loaiPhanAnh = LoaiPhanAnh.FromValue(request.LoaiPhanAnhId.Value);
                if (loaiPhanAnh == null)
                    return Result.Failure<PhanAnhResponse>(new Error("LoaiPhanAnh.Invalid", "Loại phản ánh không hợp lệ."));
            }

            // Update content in draft/withdrawn state
            var updateResult = phanAnh.Update(request.TieuDe, request.NoiDung, loaiPhanAnh, tepPhanAnhs);
            if (updateResult.IsFailure)
                return Result.Failure<PhanAnhResponse>(updateResult.Errors);

            // Submit draft/withdrawn feedback (Saved/Withdrawn -> Pending/ChoTiepNhan)
            if (request.IsSubmit)
            {
                var submitResult = phanAnh.Submit();
                if (submitResult.IsFailure)
                    return Result.Failure<PhanAnhResponse>(submitResult.Errors);
            }
        }

        // 4. Persistence
        _phanAnhCommandRepository.Update(phanAnh);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build response using query repository
        var response = await _phanAnhQueryRepository.GetByIdAsync(new GetPhanAnhByIdSpecification(phanAnh.Id), cancellationToken);

        return response != null
            ? Result.Success<PhanAnhResponse>(response)
            : Result.Failure<PhanAnhResponse>(PhanAnhErrors.NotFound);
    }
}
