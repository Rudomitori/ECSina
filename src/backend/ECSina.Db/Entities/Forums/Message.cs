namespace ECSina.Db.Entities.Forums;

public sealed class Message
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid TopicId { get; init; }
    public DataEntity? Topic { get; set; }

    public required Guid AuthorId { get; init; }
    public DataEntity? Author { get; set; }

    public required string Content { get; set; }

    public required DateTime CreatedAt { get; set; }
}
