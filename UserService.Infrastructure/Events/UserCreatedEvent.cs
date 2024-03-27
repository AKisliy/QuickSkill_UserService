using EventBus.Base.Standard;

namespace UserService.Infrastructure.Events
{
    public class UserCreatedEvent
    {
        public int UserId { get; set; }
        
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Username { get; set; } = null!;
    }
}