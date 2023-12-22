namespace ECSina.Db.Entities.Forums;

public sealed class Message
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid ForumId { get; init; }
    public DataEntity? Forum { get; set; }

    public required Guid? AuthorId { get; init; }
    public DataEntity? Author { get; set; }

    public required string Content { get; set; }

    public required DateTime CreatedAt { get; set; }
}
