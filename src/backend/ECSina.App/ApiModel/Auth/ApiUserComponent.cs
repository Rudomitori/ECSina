using System.Text.Json.Serialization;

namespace ECSina.App.ApiModel.Auth;

[JsonDerivedType(typeof(ApiUserComponent), "User")]
public class ApiUserComponent : ApiDataComponent
{
    public required string Login { get; set; }
}
