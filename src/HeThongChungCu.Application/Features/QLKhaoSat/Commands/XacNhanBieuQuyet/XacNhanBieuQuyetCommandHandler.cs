using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.XacNhanBieuQuyet;

public class XacNhanBieuQuyetCommandHandler : ICommandHandler<XacNhanBieuQuyetCommand, bool>
{
    private readonly IKhaoSatCommandRepository _khaoSatRepository;
    private readonly IBieuQuyetCuDanCommandRepository _bieuQuyetRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly IUnitOfWork _unitOfWork;

    public XacNhanBieuQuyetCommandHandler(
        IKhaoSatCommandRepository khaoSatRepository,
        IBieuQuyetCuDanCommandRepository bieuQuyetRepository,
        ICanHoCommandRepository canHoRepository,
        IMemoryCache memoryCache,
        IUnitOfWork unitOfWork)
    {
        _khaoSatRepository = khaoSatRepository;
        _bieuQuyetRepository = bieuQuyetRepository;
        _canHoRepository = canHoRepository;
        _memoryCache = memoryCache;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(XacNhanBieuQuyetCommand command, CancellationToken cancellationToken)
    {
        // 1. Cross-verify cached OTP
        var cacheKey = $"OTP_KhaoSat_{command.KhaoSatId}_{command.CanHoId}";
        if (!_memoryCache.TryGetValue<string>(cacheKey, out var cachedOtp) || 
            !string.Equals(cachedOtp, command.OtpCode, StringComparison.Ordinal))
        {
            return Result.Failure<bool>(KhaoSatErrors.InvalidOTP);
        }

        // 2. Prevent Double Voting
        var hasVoted = await _bieuQuyetRepository.HasResidentVotedAsync(command.KhaoSatId, command.CanHoId, cancellationToken);
        if (hasVoted)
        {
            _memoryCache.Remove(cacheKey); // Invalidate OTP upon fraud detection
            return Result.Failure<bool>(KhaoSatErrors.AlreadyVoted);
        }

        // 3. Fetch Campaign and validate status
        var khaoSat = await _khaoSatRepository.GetByIdAsync(command.KhaoSatId, cancellationToken);
        if (khaoSat == null)
            return KhaoSatErrors.NotFound;

        if (khaoSat.TrangThaiId != TrangThaiKhaoSat.DangDienRa)
            return Result.Failure<bool>(KhaoSatErrors.InvalidStatus);

        // 4. Fetch Apartment details to obtain area weight
        var canHo = await _canHoRepository.GetByIdAsync(command.CanHoId, cancellationToken);
        if (canHo == null)
            return CanHoErrors.NotFoundById(command.CanHoId);

        // 5. Construct domain answers list
        var answers = command.TraLois.Select(x => (x.LuaChonId, x.NoiDungTraLoiTuDo)).ToList();

        // 6. Create vote Aggregate
        var voteResult = BieuQuyetCuDan.Create(
            command.KhaoSatId,
            command.CanHoId,
            canHo.ThongSo.DienTich,
            khaoSat.CoCheTinhDiemId,
            answers,
            isOtpVerified: true);

        if (voteResult.IsFailure)
            return voteResult.Errors;

        // 7. Save Vote
        await _bieuQuyetRepository.AddAsync(voteResult.Value, cancellationToken);

        // 8. Prevent OTP Replay Attack by removing it from RAM cache
        _memoryCache.Remove(cacheKey);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
