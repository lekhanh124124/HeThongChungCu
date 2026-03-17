using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Commands.CreateCanHo;

public class CreateCanHoCommandHandler : ICommandHandler<CreateCanHoCommand, CanHoDetailResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly ITangEFRepository _tangRepository;
    private readonly ICanHoPolicy _canHoPolicy;

    public CreateCanHoCommandHandler(
        ICanHoEFRepository canHoRepository,
        ITangEFRepository tangRepository,
        ICanHoPolicy canHoPolicy)
    {
        _canHoRepository = canHoRepository;
        _tangRepository = tangRepository;
        _canHoPolicy = canHoPolicy;
    }
    public async Task<Result<CanHoDetailResponse>> Handle(CreateCanHoCommand request, CancellationToken cancellationToken)
    {
        var tang = await _tangRepository.GetByIdAsync(request.TangId, cancellationToken);
        if (tang == null)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.NotFound);

        if (tang.LoaiTangId == LoaiTang.TangHam)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.CanHoInBasement);

        var maExists = await _canHoRepository.MaCanHoExistsAsync(request.MaCanHo, cancellationToken);
        if (maExists)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.MaCanHoAlreadyExists);

        var loaiCanHo = LoaiCanHo.FromValue(request.LoaiCanHoId);
        var tinhTrangCanHo = TrangThaiCanHo.DangTrong;

        var canHo = new Domain.Entities.CanHo(
            tang.Id,
            request.MaCanHo, 
            request.TenCanHo, 
            request.DienTich, 
            request.SoPhongNgu,
            request.SoPhongTam, 
            loaiCanHo!, 
            tinhTrangCanHo,
            _canHoPolicy);

        await _canHoRepository.AddAsync(canHo, cancellationToken);
        return Result.Success(new CanHoDetailResponse
        {
            Id = canHo!.Id,
            MaCanHo = canHo.MaCanHo,
            TenCanHo = canHo.TenCanHo,
            DienTich = canHo.DienTich,
            TangId = canHo.TangId,
            TenTang = canHo.Tang?.TenTang ?? string.Empty,
            SoPhongNgu = canHo.SoPhongNgu,
            SoPhongTam = canHo.SoPhongTam,
            LoaiCanHoId = canHo.LoaiCanHoId.Value,
            TenLoaiCanHo = canHo.LoaiCanHoId.Name,
            TinhTrangCanHoId = canHo.TinhTrangCanHoId.Value,
            TenTinhTrangCanHo = canHo.TinhTrangCanHoId.Name
        });
    }
}
