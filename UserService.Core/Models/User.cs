using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set;} = null!;

        public string Username { get; set;  } = null!;

        public int? Xp { get; set; }

        public int? UserLevel { get;  }

        public string? Description { get;  }

        public string? Email { get;  }

        public string? Password { get;  }

        public string? Photo { get; }

        public string? GoalText { get; }

        public int? GoalDays { get; }

        public string? Status { get; }

        public static User Create(string firstName, string lastName, string username, string email, string password)
        {
            return new User(firstName, lastName, username, email, password);
        }
    }
}