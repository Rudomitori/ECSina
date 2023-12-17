namespace ECSina.Common.Core.Exceptions;

public class ConflictException : DomainExceptionBase
{
    public ConflictException(string message)
        : base(message) { }
}
