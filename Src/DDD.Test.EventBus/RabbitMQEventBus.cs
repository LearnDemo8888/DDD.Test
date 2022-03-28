using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {

        private readonly IEventBusStore _eventBusStore;
        private readonly IRabbitMQPersistentConnection _connection;
        private IModel _consumerChannel;
        private IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;
        private string _queueName;
        private string _exchangeName;
        public RabbitMQEventBus(IEventBusStore eventBusStore, IServiceScopeFactory serviceProviderFactory, IRabbitMQPersistentConnection connection, string exchangeName, string queueName)
        {
            _eventBusStore = eventBusStore;
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _serviceScope = serviceProviderFactory.CreateScope();
            _serviceProvider = _serviceScope.ServiceProvider;
            _exchangeName = exchangeName;
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
            this._eventBusStore.OnEventRemoved += SubsManager_OnEventRemoved; ;
        }

        private void SubsManager_OnEventRemoved(object? sender, string eventName)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: _exchangeName,
                    routingKey: eventName);

                if (_eventBusStore.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }
            }
        }

        public void Publish(string eventName, object? eventData)
        {

            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: _exchangeName, type: "direct");//声明交换机
                var body = JsonSerializer.SerializeToUtf8Bytes(eventData, eventData.GetType(), new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; //持久化
                channel.BasicPublish(exchange: _exchangeName, routingKey: eventName,
             mandatory: true, basicProperties: properties, body: body);//发布消息  
            }
        }

        public void Subscribe(string eventName, Type handlerType)
        {
            CheckHandlerType(handlerType);
            DoInternalSubscription(eventName);
            _eventBusStore.AddSubscription(eventName,handlerType);
            StartBasicConsume();
        }

        private void CheckHandlerType(Type handlerType)
        {
            if (!typeof(IIntegrationEventHandler).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException($"{handlerType} doesn't inherit from IIntegrationEventHandler", nameof(handlerType));
            }
        }
        private void StartBasicConsume()
        {

            if (_consumerChannel != null)
            {

                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);
            }
        }
        private async Task Consumer_Received(object sender, BasicDeliverEventArgs args)

        {
            var eventName = args.RoutingKey; //路由键
            var message = Encoding.UTF8.GetString(args.Body.Span);
            try
            {


               await  ProcessEvent(eventName,message);
                _consumerChannel.BasicAck(args.DeliveryTag, multiple: false); //确认消息是否处理
            }
            catch (Exception ex)
            {
                _consumerChannel.BasicReject(args.DeliveryTag, true);//失败重发
                Console.WriteLine("处理收到的消息出错" + ex);
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {


            if (_eventBusStore.HasSubscriptionsForEvent(eventName))
            {

                //得到事件处理器
                foreach (var handler in _eventBusStore.GetHandlersForEvent(eventName))
                {


   
                    var integrationEventHandler = this._serviceProvider.GetService(handler) as IIntegrationEventHandler;

                    if (integrationEventHandler == null)
                    {
                        throw new ApplicationException($"无法创建{handler}类型的服务");
                    }
                    await Task.Yield();
                    await integrationEventHandler.Handle(eventName, message);
                }

            }
            else
            {
                Debug.WriteLine($"无RabbitMQ事件订阅{eventName}");
            }
        }



        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _eventBusStore.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_connection.IsConnected)
                {
                    _connection.TryConnect();
                }

                _consumerChannel.QueueBind(queue: _queueName,
                                    exchange: _exchangeName,
                                    routingKey: eventName);
            }
        }

        public void Unsubscribe(string eventName, Type handlerType)
        {
            CheckHandlerType(handlerType);
            _eventBusStore.RemoveSubscription(eventName, handlerType);

        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
            _eventBusStore.Clear();
            this._connection.Dispose();
            this._serviceScope.Dispose();
        }


        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchange: _exchangeName,
                                    type: "direct");

            channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                /*
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();*/
                Debug.Fail(ea.ToString());
            };

            return channel;
        }
    }
}
