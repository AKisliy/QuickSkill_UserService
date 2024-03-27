namespace UserService.WebApi.Dtos
{
    public class OtherUserResponse
    {
        public string Username { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int Xp { get; set; }

        public string Photo { get; set; } = null!;

        public int Streak { get; set; }
    }
}