using MediatR;
using UserService.Core.Models;

namespace UserService.Core.Notifications
{
    public record UserChangedNotification(User user): INotification;
}