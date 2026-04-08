using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class CanHoCreatedDomainEvent : BaseEvent
{
    public CanHo CanHo { get; }

    public CanHoCreatedDomainEvent(CanHo canHo)
    {
        CanHo = canHo;
    }
}
