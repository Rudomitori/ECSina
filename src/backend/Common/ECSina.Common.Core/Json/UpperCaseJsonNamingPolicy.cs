using System.Text.Json;

namespace ECSina.Common.Core.Json;

public sealed class UpperCaseJsonNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToUpper();
}
