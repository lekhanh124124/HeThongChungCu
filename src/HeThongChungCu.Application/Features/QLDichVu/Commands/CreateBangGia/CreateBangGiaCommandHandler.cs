using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetBangGiaById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateBangGia;

public class CreateBangGiaCommandHandler : ICommandHandler<CreateBangGiaCommand, BangGiaResponse>
{
    private readonly IDichVuCommandRepository _commandRepository;
    private readonly IDichVuQueryRepository _queryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBangGiaCommandHandler(
        IDichVuCommandRepository commandRepository,
        IDichVuQueryRepository queryRepository,
        IUnitOfWork unitOfWork)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BangGiaResponse>> Handle(CreateBangGiaCommand request, CancellationToken cancellationToken)
    {
        var dichVu = await _commandRepository.GetByIdWithBangGiasAsync(request.DichVuId, cancellationToken);
        if (dichVu == null)
            return DichVuErrors.NotFoundById(request.DichVuId);

        var loaiDinhGia = LoaiDinhGia.FromValue(request.LoaiDinhGiaId)!;

        // Using DichVu domain methods to add BangGia
        if (loaiDinhGia == LoaiDinhGia.CoDinh)
        {
            dichVu.AddBangGiaCoDinh(request.TenBangGia, request.NgayApDung, request.DonGiaCoDinh ?? 0, ngayKetThuc: request.NgayKetThuc);
        }
        else if (loaiDinhGia == LoaiDinhGia.LuyTien)
        {
            dichVu.AddBangGiaLuyTien(request.TenBangGia, request.NgayApDung, request.NgayKetThuc);
            var bangGia = dichVu.BangGias.Last() as BangGiaLuyTien;
            foreach (var detail in request.GiaLuyTiens)
            {
                bangGia!.AddChiTietGia(detail.TuMuc, detail.DenMuc, detail.DonGia);
            }
        }
        else if (loaiDinhGia == LoaiDinhGia.TheoKhungGio)
        {
            dichVu.AddBangGiaKhungGio(request.TenBangGia, request.NgayApDung, request.NgayKetThuc);
            var bangGia = dichVu.BangGias.Last() as BangGiaKhungGio;
            foreach (var detail in request.GiaKhungGios)
            {
                bangGia!.AddGiaKhungGio(detail.KhungGioId, detail.DonGia);
            }
        }
        else if (loaiDinhGia == LoaiDinhGia.TheoDienTich)
        {
            dichVu.AddBangGiaLoaiCanHo(request.TenBangGia, request.NgayApDung, request.NgayKetThuc);
            var bangGia = dichVu.BangGias.Last() as BangGiaLoaiCanHo;
            foreach (var detail in request.GiaLoaiCanHos)
            {
                var loaiCanHo = detail.LoaiCanHoId.HasValue ? LoaiCanHo.FromValue(detail.LoaiCanHoId.Value) : null;
                bangGia!.AddGiaLoaiCanHo(loaiCanHo, detail.DonGia);
            }
        }
        else
        {
            return DichVuErrors.LoaiDinhGiaNotSupported;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Get the newly created BangGiaId (it will be assigned after SaveChanges)
        var newBangGia = dichVu.BangGias.OrderByDescending(x => x.Id).First();

        var result = await _queryRepository.GetBangGiaByIdAsync(new GetBangGiaByIdSpecification(newBangGia.Id), cancellationToken);

        if (result == null)
        {
            return DichVuErrors.GetBangGiaAfterActionFailed;
        }

        return result;
    }
}
