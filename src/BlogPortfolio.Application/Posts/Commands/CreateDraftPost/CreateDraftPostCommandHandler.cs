using BlogPortfolio.Application.Common.Interfaces;
using BlogPortfolio.Application.Common.Results;
using FluentValidation;

namespace BlogPortfolio.Application.Posts.Commands.CreateDraftPost
{
    public class CreateDraftPostCommandHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CreateDraftPostCommand> _validator;

        public CreateDraftPostCommandHandler(
            IPostRepository postRepository,
            IUserRepository userRepository,
            IValidator<CreateDraftPostCommand> validator)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(CreateDraftPostCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return Result<Guid>.Failure(validationResult.Errors[0].ErrorMessage);
            }

            var owner = await _userRepository.GetByIdAsync(command.OwnerId);
            
            if (owner is null)
                return Result<Guid>.Failure("User not found");

            var post = Post.CreateDraft(owner.Id);

            await _postRepository.AddAsync(post);

            return Result<Guid>.Success(post.Id);
        }
    }
}
