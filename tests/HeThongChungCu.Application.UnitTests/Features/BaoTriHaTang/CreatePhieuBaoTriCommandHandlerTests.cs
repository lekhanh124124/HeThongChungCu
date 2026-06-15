using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreatePhieuBaoTri;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;
using System.Text.Json;

namespace HeThongChungCu.Application.UnitTests.Features.BaoTriHaTang;

public class CreatePhieuBaoTriCommandHandlerTests : BaseTest
{
    private readonly IPhieuBaoTriCommandRepository _phieuBaoTriRepository;
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePhieuBaoTriCommandHandlerTests()
    {
        _phieuBaoTriRepository = CreateMock<IPhieuBaoTriCommandRepository>();
        _thietBiRepository = CreateMock<IThietBiCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_Should_UseNoiDungChecklistBanDaus_When_ProvidedInRequest()
    {
        // Arrange
        var requestChecklist = new List<string> { "Custom task 1", "Custom task 2" };
        var command = new CreatePhieuBaoTriCommand(
            MaPhieu: "PBT-001",
            ThietBiId: 1,
            HangMucBaoTriId: 2,
            NgayDuKien: DateTimeOffset.UtcNow.AddDays(2),
            HopDongDoiTacId: null,
            GhiChuXuLy: "Manual creation",
            NoiDungChecklistBanDaus: requestChecklist,
            NhanSus: null
        );

        var thietBi = ThietBi.Create("TB-001", "Device 1", "Loai 1", "Vi Tri 1", DateTimeOffset.UtcNow, null, null, null, null);
        _thietBiRepository.GetThietBiByIdAsync(command.ThietBiId, CancellationToken).Returns(thietBi);

        var standardChecklistJson = JsonSerializer.Serialize(new List<string> { "Standard task 1", "Standard task 2" });
        var hangMuc = HangMucBaoTri.Create("HM-001", "Category 1", "Desc", 60, 100000, standardChecklistJson);
        _thietBiRepository.GetHangMucByIdAsync(command.HangMucBaoTriId, CancellationToken).Returns(hangMuc);

        _phieuBaoTriRepository.MaPhieuExistsAsync(command.MaPhieu, CancellationToken).Returns(false);

        PhieuBaoTri? capturedPhieu = null;
        await _phieuBaoTriRepository.AddPhieuBaoTriAsync(Arg.Do<PhieuBaoTri>(p => capturedPhieu = p), CancellationToken);

        // When savedPhieu is re-fetched, return the captured phieu
        _phieuBaoTriRepository.GetPhieuBaoTriByIdAsync(Arg.Any<int>(), CancellationToken)
            .Returns(x => capturedPhieu);

        var handler = new CreatePhieuBaoTriCommandHandler(_phieuBaoTriRepository, _thietBiRepository, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Checklists.Should().HaveCount(2);
        result.Value.Checklists.Select(c => c.NoiDungChecklist).Should().ContainInOrder(requestChecklist);

        capturedPhieu.Should().NotBeNull();
        capturedPhieu!.Checklists.Should().HaveCount(2);
        capturedPhieu!.Checklists.Select(c => c.NoiDungChecklist).Should().ContainInOrder(requestChecklist);

        await _phieuBaoTriRepository.Received(1).AddPhieuBaoTriAsync(Arg.Any<PhieuBaoTri>(), CancellationToken);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_FallbackToChecklistTieuChuan_When_NotProvidedInRequest()
    {
        // Arrange
        var command = new CreatePhieuBaoTriCommand(
            MaPhieu: "PBT-002",
            ThietBiId: 1,
            HangMucBaoTriId: 2,
            NgayDuKien: DateTimeOffset.UtcNow.AddDays(2),
            HopDongDoiTacId: null,
            GhiChuXuLy: "Manual creation without checklist",
            NoiDungChecklistBanDaus: null,
            NhanSus: null
        );

        var thietBi = ThietBi.Create("TB-001", "Device 1", "Loai 1", "Vi Tri 1", DateTimeOffset.UtcNow, null, null, null, null);
        _thietBiRepository.GetThietBiByIdAsync(command.ThietBiId, CancellationToken).Returns(thietBi);

        var standardChecklist = new List<string> { "Standard task 1", "Standard task 2", "Standard task 3" };
        var standardChecklistJson = JsonSerializer.Serialize(standardChecklist);
        var hangMuc = HangMucBaoTri.Create("HM-001", "Category 1", "Desc", 60, 100000, standardChecklistJson);
        _thietBiRepository.GetHangMucByIdAsync(command.HangMucBaoTriId, CancellationToken).Returns(hangMuc);

        _phieuBaoTriRepository.MaPhieuExistsAsync(command.MaPhieu, CancellationToken).Returns(false);

        PhieuBaoTri? capturedPhieu = null;
        await _phieuBaoTriRepository.AddPhieuBaoTriAsync(Arg.Do<PhieuBaoTri>(p => capturedPhieu = p), CancellationToken);

        // When savedPhieu is re-fetched, return the captured phieu
        _phieuBaoTriRepository.GetPhieuBaoTriByIdAsync(Arg.Any<int>(), CancellationToken)
            .Returns(x => capturedPhieu);

        var handler = new CreatePhieuBaoTriCommandHandler(_phieuBaoTriRepository, _thietBiRepository, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Checklists.Should().HaveCount(3);
        result.Value.Checklists.Select(c => c.NoiDungChecklist).Should().ContainInOrder(standardChecklist);

        capturedPhieu.Should().NotBeNull();
        capturedPhieu!.Checklists.Should().HaveCount(3);
        capturedPhieu!.Checklists.Select(c => c.NoiDungChecklist).Should().ContainInOrder(standardChecklist);

        await _phieuBaoTriRepository.Received(1).AddPhieuBaoTriAsync(Arg.Any<PhieuBaoTri>(), CancellationToken);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }
}
