using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class User : AggregateRoot
{
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;

    public string IdCard { get; private set; } = null!;
    public DateTime Dob { get; private set; }
    public GioiTinh GioiTinhId { get; private set; } = null!;
    public string? AnhDaiDienUrl { get; private set; }
    public string DiaChi { get; private set; } = null!;


    public bool IsActive { get; private set; }

    public Role RoleId { get; private set; } = null!;

    private readonly List<Tokens> _tokens = new();
    public IReadOnlyCollection<Tokens> Tokens => _tokens.AsReadOnly();

    private readonly List<QuanHeCuTru> _quanHeCuTrus = new();
    public IReadOnlyCollection<QuanHeCuTru> QuanHeCuTrus => _quanHeCuTrus.AsReadOnly();

    private User() { } // EF Core

    public User(string username, string email, string passwordHash, string firstName, string lastName, string phoneNumber, string idCard, DateTime dob, GioiTinh gioiTinhId, string diaChi)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        IdCard = idCard;
        Dob = dob;
        GioiTinhId = gioiTinhId;
        DiaChi = diaChi;
        IsActive = true;
        RoleId = Role.Guest; // Mặc định là Guest khi tạo mới
    }

    public void UpdateProfile(string firstName, string lastName, string phoneNumber, string idCard, DateTime dob, GioiTinh gioiTinhId, string diaChi)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        IdCard = idCard;
        Dob = dob;
        GioiTinhId = gioiTinhId;
        DiaChi = diaChi;
    }

    public void UpdateAvatar(string? url)
    {
        AnhDaiDienUrl = url;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void ChangeRole(Role role)
    {
        RoleId = role;
    }

    public void AddToken(Tokens token)
    {
        _tokens.Add(token);
    }
}
