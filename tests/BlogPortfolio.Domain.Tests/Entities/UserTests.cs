using BlogPortfolio.Domain.Common;
using BlogPortfolio.Domain.Entities;
using FluentAssertions;

namespace BlogPortfolio.Domain.Tests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Creating_User_Without_Name_Should_Be_Throw_Exception()
        {
            Action act = () => new User("", "", "");

            act.Should()
               .Throw<DomainException>()
               .WithMessage("Name is required");
        }

        [Fact]
        public void Creating_User_Without_Email_Should_Be_Throw_Exception()
        {
            Action act = () => new User("Cool name", "", "");

            act.Should()
               .Throw<DomainException>()
               .WithMessage("Email is required");
        }

        [Fact]
        public void Creating_User_Without_PassWord_Should_Be_Throw_Exception()
        {
            Action act = () => new User("Cool name", "Cool email", "");

            act.Should()
               .Throw<DomainException>()
               .WithMessage("Password is required");
        }

        [Fact]
        public void Creating_User_Without_Role_Should_Be_Set_User_As_role()
        {
            var user = new User("Cool name", "Cool email", "Cool password");

            user.Role.Should().Be(Enums.UserRole.User);
        }

        [Fact]
        public void Creating_User_With_Admin_Role_Should_Be_Set_Admin_As_role()
        {
            var user = new User("Cool name", "Cool email", "Cool password", Enums.UserRole.Admin);

            user.Role.Should().Be(Enums.UserRole.Admin);
        }
    }
}
