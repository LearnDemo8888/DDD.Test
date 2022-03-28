using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    /// <summary>
    /// 事件名特性
    /// </summary>
    public class EventNameAttribute : Attribute
    {

        public string EventName { get; private set; }
        public EventNameAttribute(string eventName)
        {
            EventName = eventName;
        }
    }
}
