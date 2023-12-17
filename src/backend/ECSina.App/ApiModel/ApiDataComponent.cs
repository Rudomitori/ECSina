using System.Text.Json.Serialization;

namespace ECSina.App.ApiModel;

[JsonPolymorphic]
public abstract class ApiDataComponent
{
    public required Guid Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required Guid? CreatedById { get; set; }
}
