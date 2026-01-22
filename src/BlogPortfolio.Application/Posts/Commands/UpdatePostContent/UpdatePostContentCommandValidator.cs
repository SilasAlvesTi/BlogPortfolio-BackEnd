using FluentValidation;

namespace BlogPortfolio.Application.Posts.Commands.UpdatePostContent
{
    internal class UpdatePostContentCommandValidator : AbstractValidator<UpdatePostContentCommand>
    {
        public UpdatePostContentCommandValidator()
        {
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Content)
                .NotEmpty();
        }
    }
}
