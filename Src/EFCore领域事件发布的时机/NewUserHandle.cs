using MediatR;

namespace EFCore领域事件发布的时机
{
    public class NewUserHandle : NotificationHandler<NewUserNotification>
    {
        private readonly ILogger<NewUserHandle> _logger;

        public NewUserHandle(ILogger<NewUserHandle> logger)
        {
            _logger = logger;
        }

        protected override void Handle(NewUserNotification notification)
        {

            _logger.LogInformation($"创建用户： userName={notification.userName},time={notification.time}");
        }
    }
}
