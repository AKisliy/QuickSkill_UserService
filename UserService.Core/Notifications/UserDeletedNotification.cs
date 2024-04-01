using MediatR;

namespace UserService.Core.Notifications
{
    public record UserDeletedNotification(int id): INotification;
}