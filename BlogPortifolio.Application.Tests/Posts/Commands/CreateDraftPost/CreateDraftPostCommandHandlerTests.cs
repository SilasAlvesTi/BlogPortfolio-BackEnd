using BlogPortfolio.Application.Common.Interfaces;
using BlogPortfolio.Application.Posts.Commands.CreateDraftPost;
using BlogPortfolio.Domain.Entities;
using BlogPortfolio.Domain.Enums;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace BlogPortifolio.Application.Tests.Posts.Commands.CreateDraftPost
{
    public class CreateDraftPostCommandHandlerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IValidator<CreateDraftPostCommand> _validator;

        public CreateDraftPostCommandHandlerTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _validator = new CreateDraftPostCommandValidator();
        }

        private CreateDraftPostCommandHandler CreateHandler()
            => new(
                _postRepositoryMock.Object,
                _userRepositoryMock.Object,
                _validator
            );

        [Fact]
        public async Task Handle_Should_Fail_When_OwnerId_Is_Empty()
        {
            var handler = CreateHandler();
            var command = new CreateDraftPostCommand(Guid.Empty);

            var result = await handler.Handle(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("OwnerId is required");
        }

        [Fact]
        public async Task Handle_Should_Fail_When_User_Does_Not_Exist()
        {
            var handler = CreateHandler();
            var ownerId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(ownerId))
                .ReturnsAsync((User?)null);

            var command = new CreateDraftPostCommand(ownerId);

            var result = await handler.Handle(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("User not found");
        }

        [Fact]
        public async Task Handle_Should_Create_Draft_Post_When_Data_Is_Valid()
        {
            var handler = CreateHandler();
            var ownerId = Guid.NewGuid();

            var user = new User("Cool name", "CoolEmail@email.com", "CoolPassword");

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(ownerId))
                .ReturnsAsync(user);

            _postRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Post>()))
                .Returns(Task.CompletedTask);

            var command = new CreateDraftPostCommand(ownerId);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            _postRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Post>(p => p.Status == PostStatus.Draft)),
                Times.Once
            );
        }

    }
}
