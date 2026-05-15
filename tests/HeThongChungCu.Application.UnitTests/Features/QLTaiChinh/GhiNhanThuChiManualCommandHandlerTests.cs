using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLTaiChinh.Commands.GhiNhanThuChiManual;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.UnitTests.Features.QLTaiChinh;

public class GhiNhanThuChiManualCommandHandlerTests : BaseTest
{
    private readonly IThuChiQuyCommandRepository _thuChiQuyRepo;
    private readonly IUnitOfWork _unitOfWork;

    public GhiNhanThuChiManualCommandHandlerTests()
    {
        _thuChiQuyRepo = CreateMock<IThuChiQuyCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_Should_CreateTransactionAndSave_When_CommandIsValid()
    {
        // Arrange
        var command = new GhiNhanThuChiManualCommand
        {
            LoaiGiaoDichId = LoaiThuChi.Thu.Value,
            KhoanMucId = KhoanMucThuChi.ThuPhiQuanLy.Value,
            SoTien = 15000000m,
            NgayGiaoDich = DateTimeOffset.UtcNow,
            PhuongThucThanhToanId = PhuongThucThanhToan.ChuyenKhoan.Value,
            NguoiGiaoDich = "Lê Văn Khánh",
            ChungTuGoc = "HĐ-123456",
            GhiChu = "Thu phí quản lý thủ công căn hộ B-10.02"
        };

        var handler = new GhiNhanThuChiManualCommandHandler(_thuChiQuyRepo, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _thuChiQuyRepo.Received(1).AddAsync(Arg.Any<ThuChiQuy>(), CancellationToken);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AmountIsNegativeOrZero()
    {
        // Arrange
        var command = new GhiNhanThuChiManualCommand
        {
            LoaiGiaoDichId = LoaiThuChi.Thu.Value,
            KhoanMucId = KhoanMucThuChi.ThuPhiQuanLy.Value,
            SoTien = -500m, // Invalid negative amount
            NgayGiaoDich = DateTimeOffset.UtcNow,
            PhuongThucThanhToanId = PhuongThucThanhToan.ChuyenKhoan.Value,
            NguoiGiaoDich = "Lê Văn Khánh",
            ChungTuGoc = "HĐ-123456",
            GhiChu = "Thu phí"
        };

        var handler = new GhiNhanThuChiManualCommandHandler(_thuChiQuyRepo, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        await _thuChiQuyRepo.DidNotReceive().AddAsync(Arg.Any<ThuChiQuy>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
