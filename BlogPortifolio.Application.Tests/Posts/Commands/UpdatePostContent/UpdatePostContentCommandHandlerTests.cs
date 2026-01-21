using BlogPortfolio.Application.Common.Interfaces;
using BlogPortfolio.Application.Posts.Commands.UpdatePostContent;
using BlogPortfolio.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BlogPortfolio.Application.Tests.Commands
{
    public class UpdatePostContentCommandHandlerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly UpdatePostContentCommandHandler _handler;

        public UpdatePostContentCommandHandlerTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _handler = new UpdatePostContentCommandHandler(_postRepositoryMock.Object);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Post_Does_Not_Exist()
        {
            var command = new UpdatePostContentCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "New title",
                "New content"
            );

            _postRepositoryMock
                .Setup(r => r.GetByIdAsync(command.PostId))
                .ReturnsAsync((Post?)null);

            var result = await _handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Post not found");
        }

        [Fact]
        public async Task Should_Return_Failure_When_User_Is_Not_The_Owner()
        {
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId).Value!;
            post.UpdateContent("Title", "Content");

            var command = new UpdatePostContentCommand(
                post.Id,
                otherUserId,
                "New title",
                "New content"
            );

            _postRepositoryMock
                .Setup(r => r.GetByIdAsync(post.Id))
                .ReturnsAsync(post);

            var result = await _handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("User is not the owner of the post");
        }

        [Fact]
        public async Task Should_Update_Post_Content_When_User_Is_Owner()
        {
            var ownerId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId).Value!;
            post.UpdateContent("Old title", "Old content");

            var command = new UpdatePostContentCommand(
                post.Id,
                ownerId,
                "Updated title",
                "Updated content"
            );

            _postRepositoryMock
                .Setup(r => r.GetByIdAsync(post.Id))
                .ReturnsAsync(post);

            var result = await _handler.Handle(command);

            result.IsSuccess.Should().BeTrue();

            post.Title.Should().Be("Updated title");
            post.Content.Should().Be("Updated content");

            _postRepositoryMock.Verify(
                r => r.UpdateAsync(post),
                Times.Once
            );
        }

        [Fact]
        public async Task Should_Not_Update_When_Post_Is_Not_Draft()
        {
            var ownerId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId).Value!;
            post.UpdateContent("Title", "Content");
            post.Publish();

            var command = new UpdatePostContentCommand(
                post.Id,
                ownerId,
                "New title",
                "New content"
            );

            _postRepositoryMock
                .Setup(r => r.GetByIdAsync(post.Id))
                .ReturnsAsync(post);

            var result = await _handler.Handle(command);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Only draft posts can be edited");

            _postRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<Post>()),
                Times.Never
            );
        }
    }
}
