using BlogPortfolio.Domain.Common;
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

            var post = Post.CreateDraft(ownerId);

            post.Status.Should().Be(PostStatus.Draft);
        }

        [Fact]
        public void Publishing_Post_Without_Title_Should_Throw_Exception()
        {
            var ownerId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId);
            post.UpdateContent("", "");

            Action act = () => post.Publish();

            act.Should()
               .Throw<DomainException>()
               .WithMessage("Title is required");
        }

        [Fact]
        public void Publishing_Post_Without_Content_Should_Throw_Exception()
        {
            var ownerId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId);
            post.UpdateContent("Cool Title", "");

            Action act = () => post.Publish();

            act.Should()
               .Throw<DomainException>()
               .WithMessage("Content is required");
        }

        [Fact]
        public void Publishing_Valid_Post_Should_Change_Status_To_Published()
        {
            var ownerId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId);
            post.UpdateContent("Cool Title", "Cool Content");
            post.Publish();

            post.Status.Should().Be(PostStatus.Published);
        }

        [Fact]
        public void Published_Post_Should_Not_Be_Updated()
        {
            var ownerId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId);
            post.UpdateContent("Cool Title", "Cool Content");
            post.Publish();

            Action act = () => post.UpdateContent("New Cool Title", "New Cool Content");

            act.Should()
               .Throw<DomainException>()
               .WithMessage("Only draft posts can be edited");
        }

        [Fact]
        public void Archiving_Post_Should_Change_Status_To_Archived()
        {
            var ownerId = Guid.NewGuid();

            var post = Post.CreateDraft(ownerId);
            post.UpdateContent("Cool Title", "Cool Content");
            post.Publish();
            post.Archive();

            post.Status.Should().Be(PostStatus.Archived);
        }

    }
}
