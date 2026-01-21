using BlogPortfolio.Application.Common.Interfaces;
using BlogPortfolio.Application.Posts.Commands.PublishPost;
using BlogPortfolio.Domain.Entities;
using BlogPortfolio.Domain.Enums;
using FluentAssertions;
using Moq;

namespace BlogPortifolio.Application.Tests.Posts.Commands.PublishPost
{
    public class PublishPostCommandHandlerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly PublishPostCommandHandler _handler;

        public PublishPostCommandHandlerTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _handler = new PublishPostCommandHandler(_postRepositoryMock.Object);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Post_Does_Not_Exist()
        {
            var postId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            _postRepositoryMock
                .Setup(x => x.GetByIdAsync(postId))
                .ReturnsAsync((Post?)null);

            var command = new PublishPostCommand(postId, ownerId);

            var result = await _handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Post not found");
        }

        [Fact]
        public async Task Should_Return_Failure_When_User_Is_Not_The_Owner()
        {
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var postResult = Post.CreateDraft(ownerId);
            postResult.IsSuccess.Should().BeTrue();

            var post = postResult.Value!;

            post.UpdateContent("Cool title", "Cool content");

            _postRepositoryMock
                .Setup(x => x.GetByIdAsync(post.Id))
                .ReturnsAsync(post);

            var command = new PublishPostCommand(post.Id, anotherUserId);

            var result = await _handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("You are not allowed to publish this post");
        }


        [Fact]
        public async Task Should_Return_Failure_When_Post_Is_Invalid()
        {
            var ownerId = Guid.NewGuid();

            var postResult = Post.CreateDraft(ownerId);
            postResult.IsSuccess.Should().BeTrue();

            var post = postResult.Value!;

            _postRepositoryMock
                .Setup(x => x.GetByIdAsync(post.Id))
                .ReturnsAsync(post);

            var command = new PublishPostCommand(post.Id, ownerId);

            var result = await _handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Title is required");
        }

        [Fact]
        public async Task Should_Publish_Post_When_Data_Is_Valid()
        {
            var ownerId = Guid.NewGuid();

            var postResult = Post.CreateDraft(ownerId);
            postResult.IsSuccess.Should().BeTrue();

            var post = postResult.Value!;
            post.UpdateContent("Cool Title", "Cool Content");

            _postRepositoryMock
                .Setup(x => x.GetByIdAsync(post.Id))
                .ReturnsAsync(post);

            _postRepositoryMock
                .Setup(x => x.UpdateAsync(post))
                .Returns(Task.CompletedTask);

            var command = new PublishPostCommand(post.Id, ownerId);

            var result = await _handler.Handle(command);

            result.IsSuccess.Should().BeTrue();
            post.Status.Should().Be(PostStatus.Published);

            _postRepositoryMock.Verify(
                x => x.UpdateAsync(post),
                Times.Once
            );
        }
    }
}
