using BlogPortfolio.Application.Common.Interfaces;
using BlogPortfolio.Application.Common.Results;

namespace BlogPortfolio.Application.Posts.Commands.UpdatePostContent
{
    public class UpdatePostContentCommandHandler
    {
        private readonly IPostRepository _postRepository;

        public UpdatePostContentCommandHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Result> Handle(UpdatePostContentCommand command)
        {
            var post = await _postRepository.GetByIdAsync(command.PostId);

            if (post is null)
                return Result.Failure("Post not found");

            if (post.OwnerId != command.UserId)
                return Result.Failure("User is not the owner of the post");

            if (post.Status != Domain.Enums.PostStatus.Draft)
                return Result.Failure("Only draft posts can be edited");

            post.UpdateContent(command.Title, command.Content);

            await _postRepository.UpdateAsync(post);

            return Result.Success();
        }
    }
}
