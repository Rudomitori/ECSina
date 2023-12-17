using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ECSina.App.ApiModel;
using ECSina.Common.Core.Json;
using ECSina.Common.Swashbuckle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.OpenApi.Models;

namespace ECSina.App.Setup;

public static class MvcSetup
{
    public static WebApplicationBuilder SetupControllers(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<HttpsConnectionAdapterOptions>(options =>
        {
            options.ServerCertificate = X509Certificate2.CreateFromPemFile(
                "certs/backend.cert.pem",
                "certs/backend.key.pem"
            );

            var chain = new X509Certificate2Collection();
            chain.Add(options.ServerCertificate);
            chain.ImportFromPemFile("certs/chain.cert.pem");

            options.ServerCertificateChain = chain;
        });

        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks();
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(new UpperCaseJsonNamingPolicy())
            );

            var defaultJsonTypeInfoResolver = new DefaultJsonTypeInfoResolver();
            defaultJsonTypeInfoResolver.Modifiers.Add(InheritanceJsonTypeInfoModifier.Action);
            options.JsonSerializerOptions.TypeInfoResolver = defaultJsonTypeInfoResolver;
        });

        builder.Services.AddAutoMapper(options =>
        {
            options.AddProfile<ApiModelMapperProfile>();
        });

        return builder;
    }

    public static void UseControllersSetup(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.MapControllers();
    }

    public const string HealthCheckRoute = "/health";

    public static void UseHealthCheckSetup(this WebApplication app)
    {
        app.MapHealthChecks(HealthCheckRoute);
    }

    public static WebApplicationBuilder SetupSwagger(this WebApplicationBuilder builder)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SupportNonNullableReferenceTypes();

            options.UseOneOfForPolymorphism();

            options.SelectDiscriminatorNameUsing(_ => "$type");
            options.SelectDiscriminatorValueUsing(subType =>
            {
                var t = subType;
                while (t != typeof(object))
                {
                    var typeDiscriminator = t!
                        .GetCustomAttributes<JsonDerivedTypeAttribute>()
                        .FirstOrDefault(x => x.DerivedType == subType)
                        ?.TypeDiscriminator!.ToString();

                    if (typeDiscriminator is { })
                        return typeDiscriminator;

                    t = t!.BaseType;
                }

                return subType.Name;
            });
            options.SchemaFilter<EnumAsTypeDiscriminatorSchemaFilter>();

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                }
            );
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                }
            );
        });

        return builder;
    }

    public static void UseSwaggerSetup(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
