namespace UserService.Core.Models
{
    public class User
    {
        private User(string firstName, string lastName, string username, string email, string password) 
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Username = username;
            Password = password;
        }

        public User(){}
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set;} = null!;

        public string Username { get; set;  } = null!;

        public int Xp { get; set; }

        public int UserLevel { get; set; }

        public string? Description { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string? Photo { get; set; }

        public string? GoalText { get; set; }

        public int? GoalDays { get; set; }

        public string Status { get; set; }

        public int Streak { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public static User Create(string firstName, string lastName, string username, string email, string password)
        {
            return new User(firstName, lastName, username, email, password);
        }
    }
}