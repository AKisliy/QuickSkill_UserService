namespace UserService.Core.Models
{
    public class Badge
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int? Required { get; set; }

        public string TaskToAchieve { get; set; } = null!;

        public string? Photo { get; set; }

        public string? GrayPhoto { get; set; }
    }
}