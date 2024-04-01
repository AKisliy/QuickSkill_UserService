using MassTransit;
using MediatR;
using Shared;
using UserService.Core.Notifications;

namespace UserService.Application.NotificationHandlers
{
    public class UserDeletedNotificationHandler : INotificationHandler<UserDeletedNotification>
    {
        private readonly IPublishEndpoint _publisher;

        public UserDeletedNotificationHandler(IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }
        public async Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new UserDeletedEvent() { Id = notification.id }, cancellationToken);
            await Task.CompletedTask;
        }
    }
}