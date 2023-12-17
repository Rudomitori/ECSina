namespace ECSina.Db.Entities;

public abstract class DataComponent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid EntityId { get; init; }
    public DataEntity? Entity { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required Guid? CreatedById { get; set; }
    public DataEntity? CreatedBy { get; set; }
}
