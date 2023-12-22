namespace ECSina.Common.Core.Exceptions;

/// <summary>
/// The interface of an exception with an error code.
/// </summary>
/// <typeparam name="T">The enum with error codes.</typeparam>
public interface ICodedException<T>
    where T : unmanaged, Enum, IConvertible
{
    /// <summary>
    /// The error code.
    /// </summary>
    T Code { get; }
}

/// <summary>
/// The simple exception with an error code.
/// </summary>
/// <typeparam name="T">The enum with error codes.</typeparam>
public class CodedException<T> : Exception, ICodedException<T>
    where T : unmanaged, Enum, IConvertible
{
    /// <inheritdoc/>
    public T Code { get; }

    /// <summary>
    /// Initialize a new instance of the <see cref="CodedException{T}"/> class
    /// with a specified error message and an error code.
    /// </summary>
    /// <param name="message">The message, that describe the error.</param>
    /// <param name="code">The error code.</param>
    public CodedException(string message, T code)
        : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="CodedException{T}"/> class
    /// with a specified error message, an error code and a referrence to the inner exception,
    /// that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="code">The error code.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception,
    /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.
    /// </param>
    public CodedException(string message, T code, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}
