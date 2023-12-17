using System.Text.Json.Serialization;

namespace ECSina.App.ApiModel;

[JsonDerivedType(typeof(ApiHierarchyComponent), "Hierarchy")]
public class ApiHierarchyComponent : ApiDataComponent
{
    public Guid? ParentId { get; set; }
}
