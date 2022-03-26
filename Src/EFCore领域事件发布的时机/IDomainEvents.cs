using MediatR;

namespace EFCore领域事件发布的时机
{
    public interface IDomainEvents
    {
        /// <summary>
        /// 得到所有领域事件
        /// </summary>
        /// <returns></returns>
        IEnumerable<INotification> GetDomainEvents();

        /// <summary>
        /// 添加领域事件
        /// </summary>
        /// <param name="notification"></param>
        void AddDomainEvent(INotification notification);

        /// <summary>
        /// 清除所有领域事件
        /// </summary>
        void ClearDomainEvents();
    }
}
