using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{
    public class RabbitMqOptions
    {

        public RabbitMqConnections Connections { get; }

        public RabbitMqOptions()
        {
            Connections = new RabbitMqConnections();
        }
    }
}
