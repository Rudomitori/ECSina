using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ECSina.Common.Core.Json;

public class InheritanceJsonTypeInfoModifier
{
    public static void Action(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.PolymorphismOptions is null)
            return;

        var type = jsonTypeInfo.Type;

        var inheritors = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsAssignableTo(type));

        var jsonDerivedTypes = inheritors
            .SelectMany(x => x.GetCustomAttributes<JsonDerivedTypeAttribute>())
            .Select(
                x =>
                    x.TypeDiscriminator is int
                        ? new JsonDerivedType(x.DerivedType, (int)x.TypeDiscriminator)
                        : new JsonDerivedType(x.DerivedType, (string)x.TypeDiscriminator)
            );

        var newDerivedTypes = jsonDerivedTypes.ExceptBy(
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Select(x => x.DerivedType),
            x => x.DerivedType
        );

        foreach (var derivedType in newDerivedTypes)
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(derivedType);
    }
}
