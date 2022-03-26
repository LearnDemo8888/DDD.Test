using MediatR;

namespace EFCore领域事件发布的时机
{
    public record NewUserNotification(string userName,DateTime time): INotification;
    
}
