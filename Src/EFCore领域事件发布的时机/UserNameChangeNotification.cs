using MediatR;

namespace EFCore领域事件发布的时机
{
    public record UserNameChangeNotification(string oldUserName,string newUserName): INotification;
   
}
