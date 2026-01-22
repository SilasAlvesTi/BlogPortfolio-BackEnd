using FluentValidation;

namespace BlogPortfolio.Application.Posts.Commands.CreateDraftPost
{
    public class CreateDraftPostCommandValidator : AbstractValidator<CreateDraftPostCommand>
    {
        public CreateDraftPostCommandValidator()
        {
            RuleFor(x => x.OwnerId)
                .NotEmpty()
                .WithMessage("OwnerId is required");
        }
    }
}
