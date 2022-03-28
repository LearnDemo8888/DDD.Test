using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }



    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {

        private readonly IntegrationEventRabbitMQOptions _options;

        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private  IConnection _connection;
        private readonly object sync_root = new object();
        private readonly IServiceProvider _serviceProvider;
        private  bool _disposed;

        private readonly int _retryCount=3;

        /// <summary>
        /// /
        /// </summary>
        /// <param name="options"></param>
        /// <param name="serviceProvider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DefaultRabbitMQPersistentConnection(IntegrationEventRabbitMQOptions options, IServiceProvider serviceProvider)
        {

            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serviceProvider = serviceProvider;
            
            _logger = (ILogger<DefaultRabbitMQPersistentConnection>)_serviceProvider.GetService(typeof(ILogger<DefaultRabbitMQPersistentConnection>));
            _connectionFactory = GetConnectionFactory();
        }

        /// <summary>
        /// 这里可以使用委托外面传进来
        /// </summary>
        /// <returns></returns>
        private IConnectionFactory GetConnectionFactory()
        {

            var factory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(_options.UserName))
            {
                factory.UserName = _options.UserName;
            }

            if (!string.IsNullOrEmpty(_options.Password))
            {
                factory.Password = _options.Password;
            }

            return factory;

        }

        /// <summary>
        /// 判断是否连接
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("没有RabbitMQ连接可以执行此操作");
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.ConnectionShutdown -= OnConnectionShutdown;
                _connection.CallbackException -= OnCallbackException;
                _connection.ConnectionBlocked -= OnConnectionBlocked;
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client正在尝试连接");

         
            lock (sync_root)
            {
                _connection = _connectionFactory
                              .CreateConnection();

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    //_logger.LogInformation("RabbitMQ客户端获取到一个持久连接 '{HostName}' 并订阅失败事件", _connection.Endpoint.HostName);

                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    return false;
                }
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("RabbitMQ连接被shutdown。在贯通……");

            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("RabbitMQ连接引发异常。正在尝试重新连接...");

            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            _logger.LogWarning("RabbitMQ连接处于关闭状态。正在尝试重新连接...");

            TryConnect();
        }
    }
}
