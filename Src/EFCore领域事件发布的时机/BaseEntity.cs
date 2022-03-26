using MediatR;

namespace EFCore领域事件发布的时机
{
    public abstract class BaseEntity : IDomainEvents
    {

        private readonly IList<INotification> notifications = new List<INotification>();
        public void AddDomainEvent(INotification notification)
        {
            notifications.Add(notification);
        }

        public void ClearDomainEvents()
        {
            notifications.Clear();
        }

        public IEnumerable<INotification> GetDomainEvents()
        {
           return notifications;
        }
    }
}
