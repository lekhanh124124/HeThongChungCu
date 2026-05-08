using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.CreateKhaoSat;

public class CreateKhaoSatCommandHandler : ICommandHandler<CreateKhaoSatCommand, KhaoSatResponse>
{
    private readonly IKhaoSatCommandRepository _khaoSatCommandRepository;
    private readonly IKhaoSatQueryRepository _khaoSatQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateKhaoSatCommandHandler(
        IKhaoSatCommandRepository khaoSatCommandRepository,
        IKhaoSatQueryRepository khaoSatQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _khaoSatCommandRepository = khaoSatCommandRepository;
        _khaoSatQueryRepository = khaoSatQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<KhaoSatResponse>> Handle(CreateKhaoSatCommand command, CancellationToken cancellationToken)
    {
        // 1. Parse Enums
        var loaiKhaoSat = LoaiKhaoSat.FromValue(command.LoaiKhaoSatId);
        if (loaiKhaoSat == null)
            return Result.Failure<KhaoSatResponse>(new Error("LoaiKhaoSat.Invalid", "Loại khảo sát không hợp lệ."));

        var coChe = CoCheTinhDiemBauCu.FromValue(command.CoCheTinhDiemId);
        if (coChe == null)
            return Result.Failure<KhaoSatResponse>(new Error("CoCheTinhDiem.Invalid", "Cơ chế tính điểm bầu cử không hợp lệ."));

        // 2. Create Aggregate Root
        var campaignResult = KhaoSat.Create(
            command.TieuDe,
            command.MoTa,
            loaiKhaoSat,
            coChe,
            command.NgayBatDau,
            command.NgayKetThuc,
            command.IsAnDanh);

        if (campaignResult.IsFailure)
            return campaignResult.Errors;

        var khaoSat = campaignResult.Value;

        // 3. Add Questions and Answers
        foreach (var qDto in command.CauHois)
        {
            var options = qDto.LuaChons.Select(o => (o.NoiDungLuaChon, o.IsUngVienBQT, o.TieuSuUngVien, o.UngVienId)).ToList();
            var qResult = khaoSat.ThemCauHoi(qDto.NoiDungCauHoi, qDto.IsBatBuoc, qDto.IsMultiSelect, options);
            if (qResult.IsFailure)
                return qResult.Errors;
        }

        // 4. Persistence
        await _khaoSatCommandRepository.AddAsync(khaoSat, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Query full response details
        var response = await _khaoSatQueryRepository.GetByIdAsync(new GetKhaoSatByIdSpecification(khaoSat.Id), cancellationToken);

        return response != null
            ? Result.Success<KhaoSatResponse>(response)
            : Result.Failure<KhaoSatResponse>(KhaoSatErrors.NotFound);
    }
}
