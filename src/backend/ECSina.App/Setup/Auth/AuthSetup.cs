using System.Security.Claims;
using ECSina.Common.Core.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ECSina.App.Setup.Auth;

public static class AuthSetup
{
    public static WebApplicationBuilder SetupAuth(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<JwtTokenOptions>()
            .BindConfiguration(JwtTokenOptions.Position)
            .ValidateNrt()
            .ValidateOnStart();

        var tokenOptions = builder.Configuration.Create<JwtTokenOptions>();

        builder.Services.AddAuthorization();
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = tokenOptions.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = tokenOptions.SymmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                };
            });

        builder.Services.AddScoped(serviceProvider =>
        {
            var contextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            return contextAccessor?.HttpContext?.User ?? new ClaimsPrincipal();
        });

        return builder;
    }

    public static void UseAuthSetup(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
