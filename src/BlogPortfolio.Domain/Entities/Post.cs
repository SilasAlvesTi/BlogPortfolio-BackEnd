using BlogPortfolio.Application.Common.Results;
using BlogPortfolio.Domain.Enums;

namespace BlogPortfolio.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; private set; }
        public string? Title { get; private set; }
        public string? Content { get; private set; }
        public PostStatus Status { get; private set; }
        public Guid OwnerId { get; private set; }

        private Post(Guid ownerId)
        {
            Id = Guid.NewGuid();
            OwnerId = ownerId;
            Status = PostStatus.Draft;
        }

        public static Result<Post> CreateDraft(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
                return Result<Post>.Failure("Owner is required");

            var post = new Post(ownerId);

            return Result<Post>.Success(post);
        }

        public Result UpdateContent(string title, string content)
        {
            if (Status != PostStatus.Draft)
                return Result.Failure("Only draft posts can be edited");

            Title = title;
            Content = content;

            return Result.Success();
        }

        public Result Publish()
        {
            if (Status != PostStatus.Draft)
                return Result.Failure("Only draft posts can be published");

            if (string.IsNullOrWhiteSpace(Title))
                return Result.Failure("Title is required");

            if (string.IsNullOrWhiteSpace(Content))
                return Result.Failure("Content is required");

            Status = PostStatus.Published;

            return Result.Success();
        }

        public Result Archive()
        {
            if (Status != PostStatus.Published)
                return Result.Failure("Only published posts can be archived");

            Status = PostStatus.Archived;

            return Result.Success();
        }
    }
}
