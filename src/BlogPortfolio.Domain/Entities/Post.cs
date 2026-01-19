using BlogPortfolio.Domain.Common;
using BlogPortfolio.Domain.Enums;

public class Post
{
    public Guid Id { get; private set; }
    public string? Title { get; private set; }
    public string? Content { get; private set; }
    public PostStatus Status { get; private set; }
    public Guid OwnerId { get; private set; }

    private Post(Guid ownerId)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        Status = PostStatus.Draft;
    }

    public static Post CreateDraft(Guid ownerId)
        => new(ownerId);

    public void UpdateContent(string title, string content)
    {
        if (Status != PostStatus.Draft)
            throw new DomainException("Only draft posts can be edited");

        Title = title;
        Content = content;
    }

    public void Publish()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new DomainException("Title is required");

        if (string.IsNullOrWhiteSpace(Content))
            throw new DomainException("Content is required");

        Status = PostStatus.Published;
    }

    public void Archive()
    {
        Status = PostStatus.Archived;
    }
}
