using DDD.Test.EventBus;

namespace Test1
{
    [EventName("UserAdd")]
    public class UserAdd : IIntegrationEventHandler
    {
        public async Task Handle(string eventName, string eventData)
        {
            
            Console.WriteLine($"得到事件成功：eventName={eventName},eventData={eventData}");
            await Task.CompletedTask;
        }
    }
}
