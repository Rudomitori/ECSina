using System.Reflection;
using Microsoft.Extensions.Options;

namespace ECSina.Common.Core.Configuration;

/// <summary>
/// Implementation of <see cref="IValidateOptions{TOptions}"/> that uses nullable annotations for validation.
/// </summary>
/// <typeparam name="TOpt">The options type being validated.</typeparam>
public sealed class NrtOptionsValidator<TOpt> : IValidateOptions<TOpt>
    where TOpt : class
{
    private static readonly NullabilityInfoContext _nullabilityInfoContext = new();

    /// <param name="name">The name of the option.</param>
    public NrtOptionsValidator(string? name)
    {
        Name = name;
    }

    /// <summary>
    /// The options name.
    /// </summary>
    public string? Name { get; }

    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, TOpt options)
    {
        // Null name is used to configure all named options.
        if (Name != null && Name != name)
        {
            // Ignored if not validating this instance.
            return ValidateOptionsResult.Skip;
        }

        // Ensure options are provided to validate against
        ArgumentNullException.ThrowIfNull(options);

        var validationErrors = typeof(TOpt)
            .GetProperties()
            .Where(x => x.PropertyType is { IsClass: true } or { IsInterface: true })
            .Where(x => _nullabilityInfoContext.Create(x).ReadState is NullabilityState.NotNull)
            .Where(x => x.GetValue(options) is null)
            .Select(x => $"{typeof(TOpt).Name}.{x.Name} must be not null")
            .ToList();

        return validationErrors.Any()
            ? ValidateOptionsResult.Fail(validationErrors)
            : ValidateOptionsResult.Success;
    }
}
