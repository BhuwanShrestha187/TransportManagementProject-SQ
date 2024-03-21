using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportation_Management_System
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public UserRole UserType { get; set; }

        public User(string firstName, string lastName, string username, string password, string email, UserRole userType)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Password = password;
            Email = email;
            IsActive = true;
            UserType = userType;
        }

        public User()
        { }
    }
}
