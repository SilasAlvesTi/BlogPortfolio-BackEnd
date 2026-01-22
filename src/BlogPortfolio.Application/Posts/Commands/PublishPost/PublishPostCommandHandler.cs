using BlogPortfolio.Application.Common.Interfaces;
using BlogPortfolio.Application.Common.Results;

namespace BlogPortfolio.Application.Posts.Commands.PublishPost
{
    public class PublishPostCommandHandler
    {
        private readonly IPostRepository _postRepository;

        public PublishPostCommandHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Result> Handle(PublishPostCommand command)
        {
            var post = await _postRepository.GetByIdAsync(command.PostId);

            if (post is null)
                return Result.Failure("Post not found");

            if (post.OwnerId != command.OwnerId)
                return Result.Failure("You are not allowed to publish this post");

            var publishResult = post.Publish();

            if (!publishResult.IsSuccess)
                return publishResult;

            await _postRepository.UpdateAsync(post);

            return Result.Success();
        }
    }
}
