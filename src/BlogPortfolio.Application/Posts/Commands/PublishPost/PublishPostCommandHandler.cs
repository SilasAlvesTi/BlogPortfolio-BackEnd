using BlogPortfolio.Application.Common.Interfaces;
using BlogPortfolio.Application.Common.Results;
using BlogPortfolio.Domain.Common;
using FluentValidation;

namespace BlogPortfolio.Application.Posts.Commands.PublishPost
{
    public class PublishPostCommandHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly IValidator<PublishPostCommand> _validator;

        public PublishPostCommandHandler(
            IPostRepository postRepository,
            IValidator<PublishPostCommand> validator)
        {
            _postRepository = postRepository;
            _validator = validator;
        }

        public async Task<Result> Handle(PublishPostCommand command)
        {
            var validation = await _validator.ValidateAsync(command);

            if (!validation.IsValid)
                return Result.Failure(validation.Errors[0].ErrorMessage);

            var post = await _postRepository.GetByIdAsync(command.PostId);

            if (post is null)
                return Result.Failure("Post not found");

            if (post.OwnerId != command.OwnerId)
                return Result.Failure("You are not allowed to publish this post");

            try
            {
                post.Publish();
                await _postRepository.UpdateAsync(post);

                return Result.Success();
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
