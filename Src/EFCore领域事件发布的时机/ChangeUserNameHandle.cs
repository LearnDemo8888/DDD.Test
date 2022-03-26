using MediatR;

namespace EFCore领域事件发布的时机
{
    public class ChangeUserNameHandle : NotificationHandler<UserNameChangeNotification>
    {
        private readonly ILogger<ChangeUserNameHandle> _logger;

        public ChangeUserNameHandle(ILogger<ChangeUserNameHandle> logger)
        {
            _logger = logger;
        }
     

        protected override void Handle(UserNameChangeNotification notification)
        {
            _logger.LogInformation($"更新用户名:oldUserName={notification.oldUserName},newUserName={notification.newUserName}");
        }
    }
}
