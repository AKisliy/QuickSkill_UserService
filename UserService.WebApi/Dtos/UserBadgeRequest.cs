namespace UserService.WebApi.Dtos
{
    public class UserBadgeRequest
    {
        public int UserId { get; set; }
        public int BadgeId { get; set; }
        public int Progress { get; set; }
    }
}