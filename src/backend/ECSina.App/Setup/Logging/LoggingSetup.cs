using ECSina.Common.Core.Configuration;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;

namespace ECSina.App.Setup.Logging;

public static class LoggingSetup
{
    public static WebApplicationBuilder SetupLogging(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddOptions<SerilogSelfLogOptions>()
            .BindConfiguration(SerilogSelfLogOptions.Position)
            .ValidateNrt()
            .ValidateOnStart();

        var serilogSelfLogOptions = builder.Configuration.Create<SerilogSelfLogOptions>();

        if (serilogSelfLogOptions.IsEnabled)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(serilogSelfLogOptions.FilePath!)!);

            var streamWriter = File.Exists(serilogSelfLogOptions.FilePath)
                ? new StreamWriter(File.OpenWrite(serilogSelfLogOptions.FilePath))
                : File.CreateText(serilogSelfLogOptions.FilePath!);

            SelfLog.Enable(TextWriter.Synchronized(streamWriter));
        }

        builder.Host.UseSerilog(
            (context, provider, configuration) =>
            {
                configuration
                    .ReadFrom.Services(provider)
                    .ReadFrom.Configuration(context.Configuration);
            }
        );

        return builder;
    }

    public static void UseLoggingSetup(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (context, elapsed, exception) =>
            {
                if (
                    context.Request.Path.Value?.StartsWith(
                        MvcSetup.HealthCheckRoute,
                        StringComparison.OrdinalIgnoreCase
                    )
                    is true
                )
                    return LogEventLevel.Verbose;

                return LogEventLevel.Information;
            };

            options.EnrichDiagnosticContext = (context, httpContext) =>
            {
                context.Set("RemoteIp", httpContext.Connection.RemoteIpAddress);
            };
        });
    }
}
