using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{
    public interface IEventBusStore
    {
        void AddSubscription(string eventName, Type handlerType);
        event EventHandler<string> OnEventRemoved;

        bool IsEmpty { get; }

        void Clear();

        /// <summary>
        /// 得到处理事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
         IEnumerable<Type> GetHandlersForEvent(string eventName);

        /// <summary>
        /// 判断事件是否存在
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        bool HasSubscriptionsForEvent(string eventName);

        void RemoveSubscription(string eventName, Type handlerType);
    }
}
