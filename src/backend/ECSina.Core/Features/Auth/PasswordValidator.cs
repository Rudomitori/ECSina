using Microsoft.Extensions.Options;

namespace ECSina.Core.Features.Auth;

public sealed class PasswordValidator
{
    #region Constructor and dependencies

    private readonly Options _options;

    public PasswordValidator(IOptions<Options> options)
    {
        _options = options.Value;
    }

    #endregion

    public ErrorType? Validate(string password)
    {
        if (password.Length < _options.RequiredLength)
            return ErrorType.TooShort;

        if (_options.RequireUppercase && !password.Any(char.IsUpper))
            return ErrorType.RequiresUpper;

        if (_options.RequireLowercase && !password.Any(char.IsLower))
            return ErrorType.RequiresLower;

        if (_options.RequireDigit && !password.Any(char.IsDigit))
            return ErrorType.RequiresDigit;

        if (_options.RequireNonAlphanumeric && !password.All(char.IsLetterOrDigit))
            return ErrorType.RequiresNonAlphanumeric;

        var uniqueChars = new HashSet<char>(_options.RequiredUniqueChars);
        foreach (var ch in password)
        {
            uniqueChars.Add(ch);
            if (uniqueChars.Count >= _options.RequiredUniqueChars)
                return null;
        }

        return ErrorType.TooWeek;
    }

    public sealed class Options
    {
        /// <summary>
        /// Gets or sets the minimum length a password must be. Defaults to 8.
        /// </summary>
        public int RequiredLength { get; set; } = 8;

        /// <summary>
        /// Gets or sets the minimum number of unique characters which a password must contain. Defaults to 1.
        /// </summary>
        public int RequiredUniqueChars { get; set; } = 1;

        /// <summary>
        /// Gets or sets a flag indicating if passwords must contain a non-alphanumeric character. Defaults to false.
        /// </summary>
        /// <value>True if passwords must contain a non-alphanumeric character, otherwise false.</value>
        public bool RequireNonAlphanumeric { get; set; } = false;

        /// <summary>
        /// Gets or sets a flag indicating if passwords must contain a lower case ASCII character. Defaults to true.
        /// </summary>
        /// <value>True if passwords must contain a lower case ASCII character.</value>
        public bool RequireLowercase { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if passwords must contain a upper case ASCII character. Defaults to true.
        /// </summary>
        /// <value>True if passwords must contain a upper case ASCII character.</value>
        public bool RequireUppercase { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if passwords must contain a digit. Defaults to true.
        /// </summary>
        /// <value>True if passwords must contain a digit.</value>
        public bool RequireDigit { get; set; } = true;
    }

    public enum ErrorType
    {
        TooShort,
        RequiresNonAlphanumeric,
        RequiresDigit,
        RequiresUpper,
        RequiresLower,
        TooWeek,
    }
}
