namespace ECSina.Common.Core.Exceptions;

public class UnauthorizedException : DomainExceptionBase
{
    public UnauthorizedException(string message)
        : base(message) { }
}
