using BlogPortfolio.Application.Common.Interfaces;
using BlogPortfolio.Application.Posts.Commands.CreateDraftPost;
using BlogPortfolio.Domain.Entities;
using BlogPortfolio.Domain.Enums;
using FluentAssertions;
using Moq;

namespace BlogPortfolio.Application.Tests.Posts.Commands.CreateDraftPost
{
    public class CreateDraftPostCommandHandlerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly CreateDraftPostCommandHandler _handler;

        public CreateDraftPostCommandHandlerTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _handler = new CreateDraftPostCommandHandler(
                _postRepositoryMock.Object,
                _userRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Should_Return_Failure_When_User_Does_Not_Exist()
        {
            var ownerId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(ownerId))
                .ReturnsAsync((User?)null);

            var command = new CreateDraftPostCommand(ownerId);

            var result = await _handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("User not found");

            _postRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Post>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Should_Create_Draft_Post_When_User_Exists()
        {
            var ownerId = Guid.NewGuid();

            var user = new User(
                "Cool name",
                "cool@email.com",
                "CoolPassword"
            );

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(ownerId))
                .ReturnsAsync(user);

            _postRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Post>()))
                .Returns(Task.CompletedTask);

            var command = new CreateDraftPostCommand(ownerId);

            var result = await _handler.Handle(command);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            _postRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Post>(p =>
                    p.OwnerId == ownerId &&
                    p.Status == PostStatus.Draft
                )),
                Times.Once
            );
        }
    }
}
