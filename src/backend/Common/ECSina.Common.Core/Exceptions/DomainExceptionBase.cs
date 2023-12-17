namespace ECSina.Common.Core.Exceptions;

public abstract class DomainExceptionBase : Exception
{
    public DomainExceptionBase(string message, Exception? innerException = null)
        : base(message, innerException) { }
}
