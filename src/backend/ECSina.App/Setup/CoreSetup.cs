using ECSina.Common.Core.Clock;
using ECSina.Core.Features.Auth;
using ECSina.Core.PipelineBehaviors;
using FluentValidation;

namespace ECSina.App.Setup;

public static class CoreSetup
{
    public static WebApplicationBuilder SetupCore(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(Authenticate).Assembly);
        builder.Services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(Authenticate).Assembly);
            options.AddOpenBehavior(typeof(CommandLoggingPipelineBehaviour<,>));
            options.AddOpenBehavior(typeof(QueryLoggingPipelineBehaviour<,>));
            options.AddOpenBehavior(typeof(ExceptionHandlingBehaviour<,>));
            options.AddOpenBehavior(typeof(ValidationPipelineBehaviour<,>));
        });

        builder.Services.AddSingleton<IClock>(new Clock(TimeSpan.TicksPerMillisecond));

        builder.Services.AddSingleton<PasswordValidator>();
        builder.Services.AddOptions<PasswordValidator.Options>();

        builder.Services.AddOptions<PasswordHasher.Options>();
        builder.Services.AddSingleton<PasswordHasher>();

        return builder;
    }
}
