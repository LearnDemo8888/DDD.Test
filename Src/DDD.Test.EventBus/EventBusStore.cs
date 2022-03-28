using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{
    public class EventBusStore : IEventBusStore
    {

        private readonly Dictionary<string, List<Type>> _handlers = new Dictionary<string, List<Type>>();

        public event EventHandler<string> OnEventRemoved;

        public bool IsEmpty => !_handlers.Keys.Any();

        public void Clear() => _handlers.Clear();


        /// <summary>
        /// 添加订阅
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="handlerType">处理类型</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddSubscription(string eventName, Type handlerType)
        {
            //判断事件名是否存在
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.TryAdd(eventName, new List<Type>());
            }

            if (_handlers[eventName].Contains(handlerType))
            {
                throw new ArgumentException(
           $"处理器类型 {handlerType.Name} 已经注册 '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(handlerType);

        }

        /// <summary>
        /// 删除订阅
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handlerType"></param>
        public void RemoveSubscription(string eventName, Type handlerType)
        {

            _handlers[eventName].Remove(handlerType);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                OnEventRemoved?.Invoke(this, eventName);
            }
        }
        /// <summary>
        /// 得到处理事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public IEnumerable<Type> GetHandlersForEvent(string eventName) => _handlers[eventName];

        /// <summary>
        /// 判断事件是否存在
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);
    }
}
