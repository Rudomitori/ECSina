using ECSina.Common.Core.Exceptions;
using Hellang.Middleware.ProblemDetails;

namespace ECSina.App.Setup;

public static class ExceptionHandlingSetup
{
    public static WebApplicationBuilder SetupExceptionHandling(this WebApplicationBuilder builder)
    {
        ProblemDetailsExtensions.AddProblemDetails(
            builder.Services,
            options =>
            {
                options.IncludeExceptionDetails = (ctx, ex) => builder.Environment.IsDevelopment();

                options.MapToStatusCode<NotFoundException>(StatusCodes.Status404NotFound);
                options.MapToStatusCode<ForbiddenException>(StatusCodes.Status403Forbidden);
                options.MapToStatusCode<UnauthorizedException>(StatusCodes.Status401Unauthorized);
                options.MapToStatusCode<ConflictException>(StatusCodes.Status409Conflict);
                options.MapToStatusCode<InternalException>(
                    StatusCodes.Status500InternalServerError
                );
                options.MapToStatusCode<DomainValidationException>(StatusCodes.Status400BadRequest);
            }
        );

        return builder;
    }

    public static void UseExceptionHandlingSetup(this WebApplication app)
    {
        app.UseProblemDetails();
    }
}
