using System.Text.Json.Serialization;

namespace ECSina.App.ApiModel.Auth;

[JsonDerivedType(typeof(ApiRolesComponent), "Roles")]
public sealed class ApiRolesComponent
{
    public required bool IsAdmin { get; set; }
}
