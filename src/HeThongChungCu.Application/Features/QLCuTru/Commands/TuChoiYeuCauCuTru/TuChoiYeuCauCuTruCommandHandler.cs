using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TuChoiYeuCauCuTru;

public class TuChoiYeuCauCuTruCommandHandler : ICommandHandler<TuChoiYeuCauCuTruCommand, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruEFRepository _yeuCauRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public TuChoiYeuCauCuTruCommandHandler(
        IYeuCauCuTruEFRepository yeuCauRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(TuChoiYeuCauCuTruCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<YeuCauCuTruResponse>(UserErrors.NotFound);

        var yeuCau = await _yeuCauRepository.GetByIdAsync(request.YeuCauCuTruId, cancellationToken);
        if (yeuCau == null)
            return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.NotFoundById(request.YeuCauCuTruId));

        if (yeuCau.TrangThaiId != TrangThaiYeuCau.Pending)
            return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.BadRequest("Yêu cầu này đã được xử lý hoặc không ở trạng thái chờ."));

        if (string.IsNullOrEmpty(request.LyDo))
            return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.BadRequest("Lý do từ chối là bắt buộc."));

        var now = _dateTimeProvider.UtcNow.DateTime;

        yeuCau.Reject(userId.Value, request.LyDo, now);
        _yeuCauRepository.Update(yeuCau);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new YeuCauCuTruResponse
        {
            Id = yeuCau.Id,
            CanHoId = yeuCau.CanHoId,
            MaCanHo = yeuCau.CanHo?.MaCanHo ?? string.Empty,
            LoaiYeuCauId = yeuCau.LoaiYeuCauId.Value,
            TenLoaiYeuCau = yeuCau.LoaiYeuCauId.Name,
            QuanHeCuTruId = yeuCau.QuanHeCuTruId,
            ProposedFirstName = yeuCau.YeuCauTen,
            ProposedLastName = yeuCau.YeuCauHo,
            ProposedDob = yeuCau.YeuCauNgaySinh,
            ProposedGioiTinhId = yeuCau.YeuCauGioiTinhId,
            ProposedPhoneNumber = yeuCau.YeuCauSoDienThoai,
            ProposedLoaiQuanHeId = yeuCau.YeuCauLoaiQuanHeId,
            NoiDung = yeuCau.NoiDung,
            Reason = yeuCau.LyDo,
            TrangThaiId = yeuCau.TrangThaiId.Value,
            TenTrangThai = yeuCau.TrangThaiId.Name,
            CreatedAt = yeuCau.CreatedAt,
            ProcessedAt = yeuCau.NgayXuLy,
            ProcessedBy = yeuCau.NguoiXuLyId,
            Documents = yeuCau.Documents.Select(d => new TaiLieuResponse
            {
                Id = d.Id,
                LoaiGiayToId = d.LoaiGiayToId.Value,
                TenLoaiGiayTo = d.LoaiGiayToId.Name,
                SoGiayTo = d.SoGiayTo,
                NgayPhatHanh = d.NgayPhatHanh,
                Files = d.Files.Select(f => new TepTaiLieuResponse(f.Id, f.FileUrl, f.FileName, f.ContentType)).ToList()
            }).ToList()
        });
    }
}
