namespace UserService.WebApi.Dtos
{
    public class BadgeResponse
    {
        public string Name { get; set; } = null!;

        public int? Required { get; set; }

        public string TaskToAchieve { get; set; } = null!;

        public string? Photo { get; set; }

        public string? GrayPhoto { get; set; }

    }
}