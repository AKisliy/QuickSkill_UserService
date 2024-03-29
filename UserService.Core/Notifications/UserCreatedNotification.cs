using MediatR;
using UserService.Core.Models;

namespace UserService.Core.Notifications
{
    public record UserCreatedNotification(User User): INotification;
}