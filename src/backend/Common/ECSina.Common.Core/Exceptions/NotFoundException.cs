namespace ECSina.Common.Core.Exceptions;

public class NotFoundException : DomainExceptionBase
{
    public NotFoundException(string message)
        : base(message) { }
}
