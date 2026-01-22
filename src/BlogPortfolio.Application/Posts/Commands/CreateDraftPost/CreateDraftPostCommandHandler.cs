using BlogPortfolio.Application.Common.Interfaces;
using BlogPortfolio.Application.Common.Results;
using BlogPortfolio.Domain.Entities;

namespace BlogPortfolio.Application.Posts.Commands.CreateDraftPost
{
    public class CreateDraftPostCommandHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;

        public CreateDraftPostCommandHandler(
            IPostRepository postRepository,
            IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<Guid>> Handle(CreateDraftPostCommand command)
        {
            if (command.OwnerId == Guid.Empty)
                return Result<Guid>.Failure("Owner is required");

            var user = await _userRepository.GetByIdAsync(command.OwnerId);

            if (user is null)
                return Result<Guid>.Failure("User not found");

            var postResult = Post.CreateDraft(command.OwnerId);

            if (postResult.IsFailure)
                return Result<Guid>.Failure(postResult.Error);

            var post = postResult.Value!;

            await _postRepository.AddAsync(post);

            return Result<Guid>.Success(post.Id);
        }

    }
}
