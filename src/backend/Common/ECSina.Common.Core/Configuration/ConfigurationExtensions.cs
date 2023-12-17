using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ECSina.Common.Core.Configuration;

public static class ConfigurationExtensions
{
    public static TOptions Create<TOptions>(this IConfigurationRoot configuration)
        where TOptions : class, IPositionedOptions, new()
    {
        var options = new TOptions();
        configuration.GetSection(TOptions.Position).Bind(options);

        return options;
    }

    public static OptionsBuilder<TOptions> ValidateNrt<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder
    )
        where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            new NrtOptionsValidator<TOptions>(optionsBuilder.Name)
        );
        return optionsBuilder;
    }
}
