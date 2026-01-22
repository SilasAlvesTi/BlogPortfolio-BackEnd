using FluentValidation;

namespace BlogPortfolio.Application.Posts.Commands.PublishPost
{
    public class PublishPostCommandValidator : AbstractValidator<PublishPostCommand>
    {
        public PublishPostCommandValidator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty()
                .WithMessage("PostId is required");

            RuleFor(x => x.OwnerId)
                .NotEmpty()
                .WithMessage("OwnerId is required");
        }
    }
}
