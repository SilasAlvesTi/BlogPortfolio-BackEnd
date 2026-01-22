namespace BlogPortfolio.Application.Posts.Commands.UpdatePostContent
{
    public record UpdatePostContentCommand(
        Guid PostId,
        Guid UserId,
        string Title,
        string Content
    );
}
