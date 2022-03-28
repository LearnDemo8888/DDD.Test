using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{
    public static class ServicesCollectionExtensions
    {

        public static IServiceCollection AddEventBus(this IServiceCollection services, string queueName,
          params Assembly[] assemblies)
        {

            var types = assemblies.SelectMany(o => o.GetTypes()).Where(t => t.IsAbstract == false && t.IsAssignableTo(typeof(IIntegrationEventHandler)));
            services.AddSingleton<IEventBusStore, EventBusStore>();
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<IntegrationEventRabbitMQOptions>>().Value;
                return new DefaultRabbitMQPersistentConnection(options, sp);
            });
            foreach (var type in types)
            {

                services.AddScoped(type, type);

            }
            services.AddSingleton<IEventBus>(p =>
            {


                var busStore = p.GetRequiredService<IEventBusStore>();
                var serviceScopeFactory = p.GetRequiredService<IServiceScopeFactory>();
                var connection = p.GetRequiredService<IRabbitMQPersistentConnection>();
                var options = p.GetRequiredService<IOptions<IntegrationEventRabbitMQOptions>>().Value;
                var eventBus = new RabbitMQEventBus(busStore, serviceScopeFactory, connection, options.ExchangeName, queueName);

                foreach (var type in types)
                {


                    var eventNameAttrs = type.GetCustomAttributes<EventNameAttribute>();
                    if (eventNameAttrs.Any() == false)
                    {
                        throw new ApplicationException($"至少应该有一个 on {type}");
                    }
                    foreach (var eventNameAttr in eventNameAttrs)
                    {
                        eventBus.Subscribe(eventNameAttr.EventName, type);
                    }

                }

                return eventBus;
            });
            return services;
        }
    }
}
