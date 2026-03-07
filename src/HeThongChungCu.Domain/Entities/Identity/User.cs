using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities.Identity;

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
    public int GioiTinhId { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation property
    public int RoleId { get; private set; }

    private readonly List<Tokens> _tokens = new();
    public IReadOnlyCollection<Tokens> Tokens => _tokens.AsReadOnly();

    private User() { } // EF Core

    public User(string username, string email, string passwordHash, string firstName, string lastName, string phoneNumber, string idCard, DateTime dob, int gioiTinhId)
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
        IsActive = true;
    }

    public void UpdateProfile(string firstName, string lastName, string phoneNumber, string idCard, DateTime dob, int gioiTinhId)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        IdCard = idCard;
        Dob = dob;
        GioiTinhId = gioiTinhId;
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
        RoleId = role.Value;
    }

    public void AddToken(Tokens token)
    {
        _tokens.Add(token);
    }
}
