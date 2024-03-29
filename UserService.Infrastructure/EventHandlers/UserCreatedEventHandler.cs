using MediatR;
using Shared;

namespace UserService.Infrastructure.EventHandlers
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}