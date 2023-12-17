namespace ECSina.Db.Entities;

public sealed class HierarchyComponent : DataComponent
{
    public required Guid ParentId { get; set; }
    public DataEntity? Parent { get; set; }
}
