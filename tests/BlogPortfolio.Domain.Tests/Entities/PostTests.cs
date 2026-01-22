using BlogPortfolio.Domain.Entities;
using BlogPortfolio.Domain.Enums;
using FluentAssertions;

namespace BlogPortfolio.Domain.Tests.Entities
{
    public class PostTests
    {
        [Fact]
        public void CreateDraft_Should_Start_With_Draft_Status()
        {
            var ownerId = Guid.NewGuid();

            var result = Post.CreateDraft(ownerId);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Status.Should().Be(PostStatus.Draft);
        }

        [Fact]
        public void Publishing_Post_Without_Title_Should_Fail()
        {
            var ownerId = Guid.NewGuid();
            var post = Post.CreateDraft(ownerId).Value!;

            post.UpdateContent("", "");

            var result = post.Publish();

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Title is required");
        }

        [Fact]
        public void Publishing_Post_Without_Content_Should_Fail()
        {
            var ownerId = Guid.NewGuid();
            var post = Post.CreateDraft(ownerId).Value!;

            post.UpdateContent("Cool Title", "");

            var result = post.Publish();

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Content is required");
        }

        [Fact]
        public void Publishing_Valid_Post_Should_Change_Status_To_Published()
        {
            var ownerId = Guid.NewGuid();
            var post = Post.CreateDraft(ownerId).Value!;

            post.UpdateContent("Cool Title", "Cool Content");

            var result = post.Publish();

            result.IsSuccess.Should().BeTrue();
            post.Status.Should().Be(PostStatus.Published);
        }

        [Fact]
        public void Published_Post_Should_Not_Be_Updated()
        {
            var ownerId = Guid.NewGuid();
            var post = Post.CreateDraft(ownerId).Value!;

            post.UpdateContent("Cool Title", "Cool Content");
            post.Publish();

            var result = post.UpdateContent("New Title", "New Content");

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Only draft posts can be edited");
        }

        [Fact]
        public void Archiving_Post_Should_Change_Status_To_Archived()
        {
            var ownerId = Guid.NewGuid();
            var post = Post.CreateDraft(ownerId).Value!;

            post.UpdateContent("Cool Title", "Cool Content");
            post.Publish();

            var result = post.Archive();

            result.IsSuccess.Should().BeTrue();
            post.Status.Should().Be(PostStatus.Archived);
        }
    }
}
