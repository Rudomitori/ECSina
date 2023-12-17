using System.Text.Json.Serialization;

namespace ECSina.App.ApiModel.Forums;

[JsonDerivedType(typeof(ApiTopicComponent), "Topic")]
public class ApiTopicComponent : ApiDataComponent
{
    public required string Content { get; set; }
}
