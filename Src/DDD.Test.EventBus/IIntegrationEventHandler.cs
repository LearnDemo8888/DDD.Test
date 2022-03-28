using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{

    /// <summary>
    /// 集成事件处理者
    /// </summary>
    public  interface IIntegrationEventHandler
    {
        Task Handle(string eventName, string eventData);
    }
}
