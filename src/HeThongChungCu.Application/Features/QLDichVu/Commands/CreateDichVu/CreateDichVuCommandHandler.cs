using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateDichVu;

public class CreateDichVuCommandHandler : ICommandHandler<CreateDichVuCommand, DichVuResponse>
{
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDichVuCommandHandler(
        IDichVuCommandRepository dichVuCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _dichVuCommandRepository = dichVuCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DichVuResponse>> Handle(CreateDichVuCommand request, CancellationToken cancellationToken)
    {
        var loaiDichVu = LoaiDichVu.FromValue(request.LoaiDichVuId);
        if (loaiDichVu == null)
            return Result.Failure<DichVuResponse>(DichVuErrors.InvalidType(LoaiDichVu.GetAll().Select(x => x.Name)));

        if (await _dichVuCommandRepository.MaDichVuExistsAsync(request.MaDichVu, cancellationToken))
            return Result.Failure<DichVuResponse>(DichVuErrors.MaDichVuAlreadyExists(request.MaDichVu));

        var dichVu = new Domain.Entities.DichVu(
            request.MaDichVu,
            request.TenDichVu,
            loaiDichVu,
            request.DonViTinh,
            request.MoTa,
            request.IconId,
            request.IsBatBuoc,
            request.SoLuongToiDa);

        await _dichVuCommandRepository.AddAsync(dichVu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DichVuResponse
        {
            Id = dichVu.Id,
            MaDichVu = dichVu.MaDichVu,
            TenDichVu = dichVu.TenDichVu,
            LoaiDichVuId = dichVu.LoaiDichVuId.Value,
            LoaiDichVuTen = dichVu.LoaiDichVuId.Name,
            DonViTinh = dichVu.DonViTinh,
            MoTa = dichVu.MoTa,
            IsBatBuoc = dichVu.IsBatBuoc,
            SoLuongToiDa = dichVu.SoLuongToiDa,
            TrangThaiDichVuId = dichVu.TrangThaiId.Value,
            TrangThaiDichVuTen = dichVu.TrangThaiId.Name
        });
    }
}

