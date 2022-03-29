using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{
    public class ConnectionPool : IConnectionPool
    {
        private readonly RabbitMqOptions _options;

        protected ConcurrentDictionary<string, Lazy<IConnection>> Connections { get; }

        private bool _isDisposed;

        public ConnectionPool(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
            Connections = new();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            foreach (var connection in Connections.Values)
            {
                try
                {
                    connection.Value.Dispose();
                }
                catch
                {

                }
            }

            Connections.Clear();

        }

        public IConnection Get(string? connectionName = null)
        {
            connectionName ??= RabbitMqConnections.DefaultConnectionName;
            try
            {
                var lazyConnection = Connections.GetOrAdd(connectionName,key=> new Lazy<IConnection>(() => 
                {


                    var connection = _options.Connections.GetOrDefault(key);
                    var hostnames = connection.HostName.TrimEnd(';').Split(';');
                    return hostnames.Length == 1 ? connection.CreateConnection() : connection.CreateConnection(hostnames);

                })); 
                return lazyConnection.Value;
            }
            catch (Exception)
            {

                Connections.TryRemove(connectionName,out _);
                throw;
            }
        }
    }
}
