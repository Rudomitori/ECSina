namespace ECSina.App.ApiModel.Forums;

public sealed class ApiMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid TopicId { get; init; }
    public required Guid AuthorId { get; init; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
}
