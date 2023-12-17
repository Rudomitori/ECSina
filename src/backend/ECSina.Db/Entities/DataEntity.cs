namespace ECSina.Db.Entities;

public sealed class DataEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required DateTime CreatedAt { get; set; }

    public required Guid? CreatedById { get; set; }
    public DataEntity? CreatedBy { get; set; }

    public ICollection<DataComponent>? Components { get; set; }
}
