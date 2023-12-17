using System.Diagnostics;
using System.Security.Claims;
using ECSina.Common.Core.Exceptions;
using ECSina.Core.Base;
using ECSina.Core.Features.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECSina.Core.PipelineBehaviors;

public sealed class QueryLoggingPipelineBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : RequestBase<TResponse>
{
    #region Constructor and dependencies

    private readonly ILogger _logger;
    private readonly ClaimsPrincipal _principal;

    public QueryLoggingPipelineBehaviour(
        ILogger<QueryLoggingPipelineBehaviour<TRequest, TResponse>> logger,
        ClaimsPrincipal principal
    )
    {
        _logger = logger;
        _principal = principal;
    }

    #endregion

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestTypeName = request.GetType().Name;
        var domainClaims = _principal.GetDomainClaims();

        _logger.LogDebug(
            "Starting execute {RequestType}. UserId: {UserId}. UserLogin: {UserLogin}",
            requestTypeName,
            domainClaims.Id,
            domainClaims.Login
        );

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation(
                "{RequestType} was executed successfully. UserId: {UserId}. UserLogin: {UserLogin}. Elapsed: {Elapsed} ms",
                requestTypeName,
                domainClaims.Id,
                domainClaims.Login,
                stopwatch.ElapsedMilliseconds
            );

            return response;
        }
        catch (Exception e)
        {
            stopwatch.Stop();

            if (e is InternalException internalException)
            {
                _logger.LogError(
                    internalException.InnerException,
                    "{ExceptionType} occurred while executing {RequestType}. UserId: {UserId}. UserLogin: {UserLogin}. Elapsed: {Elapsed} ms",
                    e.GetType().Name,
                    requestTypeName,
                    domainClaims.Id,
                    domainClaims.Login,
                    stopwatch.ElapsedMilliseconds
                );
            }
            else if (e is DomainExceptionBase)
            {
                _logger.LogWarning(
                    "{ExceptionType} occurred while executing {RequestType}. UserId: {UserId}. UserLogin: {UserLogin}. Elapsed: {Elapsed} ms",
                    e.GetType().Name,
                    requestTypeName,
                    domainClaims.Id,
                    domainClaims.Login,
                    stopwatch.ElapsedMilliseconds
                );
            }

            throw;
        }
    }
}
