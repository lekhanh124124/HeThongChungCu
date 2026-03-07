using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Events;

public class UserRegisteredEvent : BaseEvent
{
    public int UserId { get; }
    public string Username { get; }

    public UserRegisteredEvent(int userId, string username)
    {
        UserId = userId;
        Username = username;
    }
}
