using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{
    public interface IConnectionPool : IDisposable
    {
        IConnection Get(string? connectionName = null);
    }
}
