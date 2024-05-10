using AutoMapper;
using MassTransit;
using MediatR;
using Shared;
using UserService.Core.Notifications;

namespace UserService.Application.NotificationHandlers
{
    public class UserChangedNotificationHandler : INotificationHandler<UserChangedNotification>
    {
        private IPublishEndpoint _publisher;
        private IMapper _mapper;

        public UserChangedNotificationHandler(IPublishEndpoint publisher, IMapper mapper)
        {
            _publisher = publisher;
            _mapper = mapper;
        }
        public async Task Handle(UserChangedNotification notification, CancellationToken cancellationToken)
        {
            var changedUser = _mapper.Map<UserChangedEvent>(notification.user);
            await _publisher.Publish(changedUser);
            await Task.CompletedTask;
        }
    }
}