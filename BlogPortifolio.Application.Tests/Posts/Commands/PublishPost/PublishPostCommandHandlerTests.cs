using BlogPortfolio.Application.Common.Interfaces;
using BlogPortfolio.Application.Posts.Commands.CreateDraftPost;
using BlogPortfolio.Application.Posts.Commands.PublishPost;
using BlogPortfolio.Domain.Enums;
using FluentAssertions;
using FluentValidation;
using Moq;
using System.Threading.Tasks;

namespace BlogPortifolio.Application.Tests.Posts.Commands.PublishPost
{
    public class PublishPostCommandHandlerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly IValidator<PublishPostCommand> _validator;

        public PublishPostCommandHandlerTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _validator = new PublishPostCommandValidator();
        }

        private PublishPostCommandHandler CreateHandler()
            => new(
                _postRepositoryMock.Object,
                _validator
            );

        [Fact]
        public async Task Handle_Should_Fail_When_PostId_Is_Empty()
        {
            var handler = CreateHandler();
            var command = new PublishPostCommand(Guid.Empty, Guid.NewGuid());

            var result = await handler.Handle(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("PostId is required");
        }

        [Fact]
        public async Task Handle_Should_Fail_When_OwnerId_Is_Empty()
        {
            var handler = CreateHandler();
            var command = new PublishPostCommand(Guid.NewGuid(), Guid.Empty);

            var result = await handler.Handle(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("OwnerId is required");
        }

        [Fact]
        public async Task Handle_Should_Fail_When_Post_Does_Not_Exist()
        {
            var handler = CreateHandler();
            var postId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            _postRepositoryMock
                .Setup(x => x.GetByIdAsync(postId))
                .ReturnsAsync((Post?) null);

            var command = new PublishPostCommand(postId, ownerId);

            var result = await handler.Handle(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Post not found");
        }

        [Fact]
        public async Task Handle_Should_Fail_When_User_Is_Not_The_Owner()
        {
            var handler = CreateHandler();
            var postId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId);
            post.UpdateContent("Coll title", "Cool content");

            _postRepositoryMock
                .Setup(x => x.GetByIdAsync(postId))
                .ReturnsAsync(post);

            var command = new PublishPostCommand(postId, anotherUserId);

            var result = await handler.Handle(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("You are not allowed to publish this post");
        }

        [Fact]
        public async Task Handle_Should_Fail_When_Post_Is_Invalid()
        {
            var handler = CreateHandler();
            var postId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId);

            _postRepositoryMock
                .Setup(x => x.GetByIdAsync(postId))
                .ReturnsAsync(post);

            var command = new PublishPostCommand(postId, ownerId);

            var result = await handler.Handle(command);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Title is required");
        }

        [Fact]
        public async Task Handle_Should_Publish_Post_When_Data_Is_Valid()
        {
            var handler = CreateHandler();
            var postId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId);
            post.UpdateContent("Cool Title", "Cool Content");

            _postRepositoryMock
                .Setup(x => x.GetByIdAsync(postId))
                .ReturnsAsync(post);

            _postRepositoryMock
                .Setup(x => x.UpdateAsync(post))
                .Returns(Task.CompletedTask);

            var command = new PublishPostCommand(postId, ownerId);

            var result = await handler.Handle(command);

            result.IsSuccess.Should().BeTrue();
            post.Status.Should().Be(PostStatus.Published);

            _postRepositoryMock.Verify(
                x => x.UpdateAsync(post),
                Times.Once
            );
        }

    }
}
