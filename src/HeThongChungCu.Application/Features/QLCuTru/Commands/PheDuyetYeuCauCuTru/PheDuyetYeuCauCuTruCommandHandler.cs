using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.PheDuyetYeuCauCuTru;

public class PheDuyetYeuCauCuTruCommandHandler : ICommandHandler<PheDuyetYeuCauCuTruCommand, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruEFRepository _yeuCauRepository;
    private readonly INguoiDungEFRepository _userRepository;
    private readonly IQuanHeCuTruEFRepository _quanHeCuTruRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public PheDuyetYeuCauCuTruCommandHandler(
        IYeuCauCuTruEFRepository yeuCauRepository,
        INguoiDungEFRepository userRepository,
        IQuanHeCuTruEFRepository quanHeCuTruRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _userRepository = userRepository;
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(PheDuyetYeuCauCuTruCommand request, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId;
        if (adminId == null)
            return Result.Failure<YeuCauCuTruResponse>(UserErrors.NotFound);

        var yeuCau = await _yeuCauRepository.GetByIdAsync(request.YeuCauCuTruId, cancellationToken);
        if (yeuCau == null)
            return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.NotFoundById(request.YeuCauCuTruId));

        if (yeuCau.TrangThaiId != TrangThaiYeuCau.Pending)
            return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.BadRequest("Yêu cầu này đã được xử lý hoặc không ở trạng thái chờ."));

        var now = _dateTimeProvider.UtcNow.DateTime;

        // Logic Phê duyệt
        if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Them)
        {
            // 1. Create User
            var newUser = new NguoiDung(
                yeuCau.YeuCauTen!,
                yeuCau.YeuCauHo!,
                yeuCau.YeuCauNgaySinh ?? DateTime.MinValue,
                GioiTinh.FromValue(yeuCau.YeuCauGioiTinhId ?? 1, null)!,
                string.Empty,
                soDienThoai: yeuCau.YeuCauSoDienThoai);

            // 2. Add Documents if any
            foreach (var docReq in yeuCau.Documents)
            {
                var newDoc = new TaiLieuNguoiDung(
                    null,
                    docReq.LoaiGiayToId,
                    docReq.SoGiayTo,
                    docReq.NgayPhatHanh,
                    docReq.Files);
                newUser.AddDocument(newDoc);
            }

            await _userRepository.AddAsync(newUser, cancellationToken);

            // 3. Create Residency Relation
            var loaiQuanHe = LoaiQuanHeCuTru.FromValue(yeuCau.YeuCauLoaiQuanHeId!.Value, null);
            var existingRelations = await _quanHeCuTruRepository.GetByCanHoIdAsync(yeuCau.CanHoId, cancellationToken);
            var quanHe = new QuanHeCuTru(yeuCau.CanHoId, newUser.Id, loaiQuanHe!, now, existingRelations);

            await _quanHeCuTruRepository.AddAsync(quanHe, cancellationToken);
        }
        else if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Sua)
        {
            var relation = await _quanHeCuTruRepository.GetByIdAsync(yeuCau.QuanHeCuTruId!.Value, cancellationToken);
            if (relation == null)
                return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.NotFoundById(yeuCau.QuanHeCuTruId.Value));

            var user = await _userRepository.GetByIdAsync(relation.NguoiDungId, cancellationToken);
            if (user == null)
                return Result.Failure<YeuCauCuTruResponse>(UserErrors.NotFound);

            if (yeuCau.YeuCauLoaiQuanHeId.HasValue)
            {
                relation.ThayDoiLoaiQuanHe(LoaiQuanHeCuTru.FromValue(yeuCau.YeuCauLoaiQuanHeId.Value, null)!);
                _quanHeCuTruRepository.Update(relation);
            }

            foreach (var docReq in yeuCau.Documents)
            {
                var newDoc = new TaiLieuNguoiDung(
                    user.Id,
                    docReq.LoaiGiayToId,
                    docReq.SoGiayTo,
                    docReq.NgayPhatHanh,
                    docReq.Files);
                user.AddDocument(newDoc);
            }

            _userRepository.Update(user);
        }
        else if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Xoa)
        {
            var relation = await _quanHeCuTruRepository.GetByIdAsync(yeuCau.QuanHeCuTruId!.Value, cancellationToken);
            if (relation != null)
            {
                relation.KetThucCuTru(now);
                _quanHeCuTruRepository.Update(relation);
            }
        }

        yeuCau.Approve(adminId.Value, now);
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
