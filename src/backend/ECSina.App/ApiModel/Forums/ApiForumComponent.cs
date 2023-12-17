using System.Text.Json.Serialization;

namespace ECSina.App.ApiModel.Forums;

[JsonDerivedType(typeof(ApiForumComponent), "Forum")]
public class ApiForumComponent : ApiDataComponent { }
