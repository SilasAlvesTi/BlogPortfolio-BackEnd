namespace BlogPortfolio.Application.Posts.Commands.PublishPost
{
    public record PublishPostCommand(Guid PostId, Guid OwnerId);
}
