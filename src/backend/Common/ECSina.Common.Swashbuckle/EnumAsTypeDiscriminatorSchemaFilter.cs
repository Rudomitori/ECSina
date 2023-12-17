using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ECSina.Common.Swashbuckle;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class EnumAsTypeDiscriminatorSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var discriminator = schema.Discriminator;

        if (discriminator is null)
            return;

        schema.Properties[discriminator.PropertyName].Enum = discriminator
            .Mapping.Select(x => new OpenApiString(x.Key))
            .Cast<IOpenApiAny>()
            .ToList();
    }
}
