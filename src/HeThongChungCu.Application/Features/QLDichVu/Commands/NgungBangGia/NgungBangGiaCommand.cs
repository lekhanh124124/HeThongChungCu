using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.NgungBangGia;

public sealed record NgungBangGiaCommand(int Id) : ICommand<BangGiaResponse>;

internal sealed class NgungBangGiaCommandHandler : ICommandHandler<NgungBangGiaCommand, BangGiaResponse>
{
    private readonly IBangGiaEFRepository _bangGiaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NgungBangGiaCommandHandler(IBangGiaEFRepository bangGiaRepository, IUnitOfWork unitOfWork)
    {
        _bangGiaRepository = bangGiaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BangGiaResponse>> Handle(NgungBangGiaCommand request, CancellationToken cancellationToken)
    {
        var bangGia = await _bangGiaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (bangGia is null)
        {
            return Result.Failure<BangGiaResponse>(new Error("BangGia.NotFound", "Không tìm thấy bảng giá."));
        }

        bangGia.Deactivate();

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
