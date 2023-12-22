using System.Text.Json.Serialization;

namespace ECSina.App.ApiModel.Auth;

[JsonDerivedType(typeof(ApiPasswordComponent), "Password")]
public sealed class ApiPasswordComponent { }
