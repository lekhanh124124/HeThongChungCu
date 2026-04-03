using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Events;

public class NhanVienCreatedEvent : BaseEvent
{
    public string Email { get; }
    public string FullName { get; }
    public string UserName { get; }
    public string Password { get; }

    public NhanVienCreatedEvent(string email, string fullName, string userName, string password)
    {
        Email = email;
        FullName = fullName;
        UserName = userName;
        Password = password;
    }
}
