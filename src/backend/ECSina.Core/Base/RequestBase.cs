using MediatR;

namespace ECSina.Core.Base;

public abstract class RequestBase<TResponse> : IRequest<TResponse> { }
