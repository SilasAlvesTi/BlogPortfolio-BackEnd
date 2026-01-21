using BlogPortfolio.Domain.Common;
using BlogPortfolio.Domain.Enums;

namespace BlogPortfolio.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public UserRole Role { get; private set; }

        public User(
            string name,
            string email,
            string password,
            UserRole role = UserRole.User)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new DomainException("Password is required");

            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }

        public bool IsAdmin()
            => Role == UserRole.Admin;
    }
}
