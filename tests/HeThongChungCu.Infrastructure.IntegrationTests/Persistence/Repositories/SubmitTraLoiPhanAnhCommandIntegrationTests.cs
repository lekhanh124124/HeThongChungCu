using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLPhanAnh.Commands.SubmitTraLoiPhanAnh;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;
using HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories;

public class SubmitTraLoiPhanAnhCommandIntegrationTests : BaseIntegrationTest
{
    private readonly YeuCauPhanAnhCommandRepository _commandRepository;
    private readonly YeuCauPhanAnhQueryRepository _queryRepository;
    private readonly ICurrentUserService _currentUserService;

    public SubmitTraLoiPhanAnhCommandIntegrationTests() : base()
    {
        _commandRepository = new YeuCauPhanAnhCommandRepository(DbContext);
        _queryRepository = new YeuCauPhanAnhQueryRepository(DbContext);
        _currentUserService = Substitute.For<ICurrentUserService>();
    }

    private async Task<(CanHo CanHo, YeuCauPhanAnh Request)> CreateDataAsync()
    {
        // 1. Create building, floor, apartment
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "Tòa nhà Chat Test", "C", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang(Guid.NewGuid().ToString()[..10], "Tầng 3", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "Căn Hộ C3-01", 85.0m, 2, 1, LoaiCanHo.Standard, TrangThaiCanHo.DaBanGiao);
        await DbContext.CanHos.AddAsync(canHo);
        await DbContext.SaveChangesAsync();

        // 2. Create YeuCauPhanAnh (Complaint Request)
        var phanAnhResult = YeuCauPhanAnh.Create(
            canHo.Id,
            "Hỏng bóng đèn hành lang",
            "Bóng đèn hành lang tầng 3 bị cháy cần thay thế",
            LoaiPhanAnh.HaTangKyThuat,
            isSubmit: true
        );
        phanAnhResult.IsSuccess.Should().BeTrue();
        var phanAnh = phanAnhResult.Value;

        await DbContext.YeuCauPhanAnhs.AddAsync(phanAnh);
        await DbContext.SaveChangesAsync();

        return (canHo, phanAnh);
    }

    [Fact]
    public async Task Handle_Should_AddReplyAsResident_When_CurrentUserIsResident()
    {
        // Arrange
        var (_, phanAnh) = await CreateDataAsync();

        // Mock current user as a resident (no staff/manager/admin roles)
        _currentUserService.UserId.Returns(100);
        _currentUserService.Roles.Returns(new List<string> { Role.Resident.Name });

        var handler = new SubmitTraLoiPhanAnhCommandHandler(
            _commandRepository,
            _queryRepository,
            _currentUserService,
            DbContext
        );

        var command = new SubmitTraLoiPhanAnhCommand
        {
            PhanAnhId = phanAnh.Id,
            NoiDung = "Cư dân gửi lời nhắn: Tôi đã thấy thợ bảo trì qua nhưng chưa sửa được"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        // Fetch from DB to verify reply and ticket status
        var updatedPhanAnh = await _commandRepository.GetByIdWithRepliesAsync(phanAnh.Id);
        updatedPhanAnh.Should().NotBeNull();
        updatedPhanAnh!.TrangThaiPhanAnhId.Should().Be(TrangThaiPhanAnh.CuDanPhanHoi);
        updatedPhanAnh.TraLoiPhanAnhs.Should().NotBeEmpty();
        
        var reply = updatedPhanAnh.TraLoiPhanAnhs.Last();
        reply.NoiDung.Should().Be(command.NoiDung);
        reply.IsNhanVien.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_AddReplyAsStaff_When_CurrentUserIsStaff()
    {
        // Arrange
        var (_, phanAnh) = await CreateDataAsync();

        // Mock current user as a staff member (has "Staff" role)
        _currentUserService.UserId.Returns(200);
        _currentUserService.Roles.Returns(new List<string> { Role.Staff.Name });

        var handler = new SubmitTraLoiPhanAnhCommandHandler(
            _commandRepository,
            _queryRepository,
            _currentUserService,
            DbContext
        );

        var command = new SubmitTraLoiPhanAnhCommand
        {
            PhanAnhId = phanAnh.Id,
            NoiDung = "Ban quản lý phản hồi: Chúng tôi đang cử nhân viên kỹ thuật qua xử lý ngay"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        // Fetch from DB to verify reply and ticket status
        var updatedPhanAnh = await _commandRepository.GetByIdWithRepliesAsync(phanAnh.Id);
        updatedPhanAnh.Should().NotBeNull();
        updatedPhanAnh!.TrangThaiPhanAnhId.Should().Be(TrangThaiPhanAnh.CSKHPhanHoi);
        updatedPhanAnh.TraLoiPhanAnhs.Should().NotBeEmpty();
        
        var reply = updatedPhanAnh.TraLoiPhanAnhs.Last();
        reply.NoiDung.Should().Be(command.NoiDung);
        reply.IsNhanVien.Should().BeTrue();
    }
}
