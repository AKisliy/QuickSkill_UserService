using AutoMapper;
using MassTransit;
using MediatR;
using Shared;
using UserService.Core.Notifications;

namespace UserService.Infrastructure.NotificationHandlers
{
    public class UserCreatedNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        private readonly IPublishEndpoint _publisher;
        private readonly IMapper _mapper;

        public UserCreatedNotificationHandler(IPublishEndpoint publisher, IMapper mapper)
        {
            _publisher = publisher;
            _mapper = mapper;
        }
        public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            var newUser = _mapper.Map<UserCreatedEvent>(notification.User);
            await _publisher.Publish(newUser, cancellationToken);
            await Task.CompletedTask;
        }
    }
}