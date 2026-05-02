using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.RemoveTepThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence;
using HeThongChungCu.Infrastructure.Persistence.Repositories;
using HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;
using HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;
using NSubstitute;
using Xunit;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories;

public class RemoveTepThiCongIntegrationTests : BaseIntegrationTest
{
    private readonly YeuCauThiCongCommandRepository _commandRepository;
    private readonly YeuCauThiCongQueryRepository _queryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RemoveTepThiCongIntegrationTests() : base()
    {
        _commandRepository = new YeuCauThiCongCommandRepository(DbContext);
        _queryRepository = new YeuCauThiCongQueryRepository(DbContext);
        _unitOfWork = DbContext;

        _currentUserService = Substitute.For<ICurrentUserService>();
        _currentUserService.UserId.Returns(1); 
    }

    private async Task<(CanHo, YeuCauThiCong)> CreateDependenciesAsync()
    {
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "Tòa nhà 01", "A", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang(Guid.NewGuid().ToString()[..10], "Tầng 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "Căn Hộ A1-01", 100, 2, 2, LoaiCanHo.Studio, TrangThaiCanHo.ChuaBanGiao);
        await DbContext.CanHos.AddAsync(canHo);
        await DbContext.SaveChangesAsync();

        var yctc = YeuCauThiCong.Create(
            canHoId: canHo.Id,
            hangMucThiCong: "Sửa phòng khách",
            duKienBatDau: DateTimeOffset.Now,
            duKienKetThuc: DateTimeOffset.Now.AddDays(7),
            noiDung: "Đập tường",
            tenDonViThiCong: "Công ty ABC",
            nguoiDaiDien: "Nguyen Van A",
            soDienThoaiDaiDien: "0987654321"
        );

        yctc.AddTep(new TepYeuCauThiCong("file1.pdf", "https://blob.com/file1.pdf", 1024, "application/pdf"));
        yctc.AddTep(new TepYeuCauThiCong("file2.pdf", "https://blob.com/file2.pdf", 2048, "application/pdf"));

        await _commandRepository.AddAsync(yctc);
        await DbContext.SaveChangesAsync();

        return (canHo, yctc);
    }

    [Fact]
    public async Task Handle_Should_MarkTepAsDeleted_And_QueryShouldNotReturnIt()
    {
        // Arrange
        var (_, yctc) = await CreateDependenciesAsync();
        var tepToRemove = yctc.TepYeuCauThiCongs.First();

        var handler = new RemoveTepThiCongCommandHandler(
            _commandRepository, 
            _queryRepository, 
            _currentUserService, 
            _unitOfWork);

        var command = new RemoveTepThiCongCommand(yctc.Id, tepToRemove.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // 1. Kiểm tra trong DbContext (Dữ liệu vẫn còn nhưng IsDeleted = true)
        var tepInDb = await DbContext.TepTaiLieus.FindAsync(tepToRemove.Id);
        tepInDb.Should().NotBeNull();
        tepInDb!.IsDeleted.Should().BeTrue();

        // 2. Kiểm tra thông qua Query Repository (Không được trả về nữa do bộ lọc IsDeleted)
        var spec = new GetYeuCauThiCongByIdSpecification(yctc.Id);
        var queryResult = await _queryRepository.GetByIdAsync(spec);

        queryResult.Should().NotBeNull();
        
        // Ban đầu thêm 2 tệp, xóa 1 tệp thì danh sách truy vấn được chỉ còn 1
        queryResult!.DanhSachTep.Should().HaveCount(1);
        
        // Tệp bị xóa không được có mặt trong danh sách
        queryResult.DanhSachTep.Any(x => x.Id == tepToRemove.Id).Should().BeFalse();
    }
}
