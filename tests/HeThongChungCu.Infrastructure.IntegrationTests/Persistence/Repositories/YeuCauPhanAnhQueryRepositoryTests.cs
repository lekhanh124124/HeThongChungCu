using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhList;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;
using Xunit;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories;

public class YeuCauPhanAnhQueryRepositoryTests : BaseIntegrationTest
{
    private readonly YeuCauPhanAnhQueryRepository _repository;

    public YeuCauPhanAnhQueryRepositoryTests() : base()
    {
        _repository = new YeuCauPhanAnhQueryRepository(DbContext);
    }

    private async Task<(YeuCauPhanAnh AssignedRequest, YeuCauPhanAnh UnassignedRequest, NhanVien Staff)> CreateDataAsync()
    {
        // 1. Create building, floor, apartment
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "Tòa nhà Phản Ánh", "P", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang(Guid.NewGuid().ToString()[..10], "Tầng 2", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "Căn Hộ P2-01", 90.0m, 2, 2, LoaiCanHo.Standard, TrangThaiCanHo.CoCuDan);
        await DbContext.CanHos.AddAsync(canHo);
        await DbContext.SaveChangesAsync();

        // 2. Create Staff User and NhanVien
        var staffUser = NguoiDung.CreateNguoiDung(
            "Bình", "Nguyễn", DateTimeOffset.Now.AddYears(-28), GioiTinh.Nam, "Văn Phòng Ban Quản Lý"
        );
        await DbContext.NguoiDung.AddAsync(staffUser);
        await DbContext.SaveChangesAsync();

        var nhanVien = NhanVien.CreateNhanVien(staffUser.Id, LoaiNhanVien.KyThuat, "NV001", DateTimeOffset.Now.AddYears(-1));
        await DbContext.NhanViens.AddAsync(nhanVien);
        await DbContext.SaveChangesAsync();

        // 3. Create YeuCauPhanAnh 1: Assigned to NhanVien
        var assignedResult = YeuCauPhanAnh.Create(
            canHo.Id,
            "Hỏng thang máy block P",
            "Thang máy block P di chuyển rất chậm và rung lắc",
            LoaiPhanAnh.HaTangKyThuat,
            isSubmit: true
        );
        assignedResult.IsSuccess.Should().BeTrue();
        var assignedRequest = assignedResult.Value;

        // Simulate approval and assignment
        assignedRequest.TiepNhanVaPhanCong(nhanVien.Id, DateTimeOffset.Now);

        await DbContext.YeuCauPhanAnhs.AddAsync(assignedRequest);
        await DbContext.SaveChangesAsync();

        // 4. Create YeuCauPhanAnh 2: Unassigned
        var unassignedResult = YeuCauPhanAnh.Create(
            canHo.Id,
            "Hành lang tầng 2 bẩn",
            "Có rác sinh hoạt để ngoài hành lang tầng 2 bốc mùi",
            LoaiPhanAnh.VeSinhMoitruong,
            isSubmit: true
        );
        unassignedResult.IsSuccess.Should().BeTrue();
        var unassignedRequest = unassignedResult.Value;

        await DbContext.YeuCauPhanAnhs.AddAsync(unassignedRequest);
        await DbContext.SaveChangesAsync();

        return (assignedRequest, unassignedRequest, nhanVien);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnOnlyAssigned_When_FilteredByNguoiXuLyId()
    {
        // Arrange
        var (assigned, _, staff) = await CreateDataAsync();

        var spec = new GetPhanAnhListSpecification(
            loaiPhanAnhId: null,
            trangThaiPhanAnhId: null,
            canHoId: null,
            keyword: null,
            ngayTaoTu: null,
            ngayTaoDen: null,
            sortCol: null,
            isAsc: null,
            pageNumber: 1,
            pageSize: 10,
            nguoiXuLyId: staff.Id
        );

        // Act
        var result = await _repository.GetAllAsync(spec);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Should().HaveCount(1);
        result.Items.First().Id.Should().Be(assigned.Id);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnAll_When_NguoiXuLyIdIsNull()
    {
        // Arrange
        var (assigned, unassigned, _) = await CreateDataAsync();

        var spec = new GetPhanAnhListSpecification(
            loaiPhanAnhId: null,
            trangThaiPhanAnhId: null,
            canHoId: null,
            keyword: null,
            ngayTaoTu: null,
            ngayTaoDen: null,
            sortCol: null,
            isAsc: null,
            pageNumber: 1,
            pageSize: 10,
            nguoiXuLyId: null
        );

        // Act
        var result = await _repository.GetAllAsync(spec);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Select(i => i.Id).Should().Contain(new[] { assigned.Id, unassigned.Id });
    }
}
