using ECSina.Common.Core.Exceptions;
using MediatR;

namespace ECSina.Core.PipelineBehaviors;

public class ExceptionHandlingBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return next();
        }
        catch (Exception e)
        {
            if (e is DomainExceptionBase)
                throw;

            throw new InternalException("Unexpected exception occurred", e);
        }
    }
}
