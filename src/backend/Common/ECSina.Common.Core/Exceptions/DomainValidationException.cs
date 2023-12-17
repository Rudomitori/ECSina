namespace ECSina.Common.Core.Exceptions;

public class DomainValidationException : DomainExceptionBase
{
    public DomainValidationException(string message)
        : base(message) { }
}
