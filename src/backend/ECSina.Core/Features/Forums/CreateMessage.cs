using System.Security.Claims;
using ECSina.Common.Core.Clock;
using ECSina.Common.Core.Exceptions;
using ECSina.Core.Base;
using ECSina.Core.Features.Auth;
using ECSina.Db;
using ECSina.Db.Entities;
using ECSina.Db.Entities.Forums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECSina.Core.Features.Forums;

public sealed class CreateMessage : CommandBase<CreateMessage.Response>
{
    public required Guid ForumId { get; set; }
    public required string Content { get; set; }

    public sealed class Response
    {
        public required Message Message { get; set; }
    }

    public sealed class Handler : IRequestHandler<CreateMessage, Response>
    {
        #region Constructor and dependencies

        private readonly AppDbContext _dbContext;
        private readonly IClock _clock;
        private readonly ClaimsPrincipal _principal;

        public Handler(AppDbContext dbContext, ClaimsPrincipal principal, IClock clock)
        {
            _dbContext = dbContext;
            _principal = principal;
            _clock = clock;
        }

        #endregion

        public async Task<Response> Handle(
            CreateMessage request,
            CancellationToken cancellationToken
        )
        {
            if (_principal.Identity is not { IsAuthenticated: true })
                throw new ForbiddenException("Only an authenticated user can create message");

            var now = _clock.UtcNow;
            var domainClaims = _principal.GetDomainClaims();

            var entity = await _dbContext
                .Set<DataEntity>()
                .Include(x => x.Components)
                .FirstOrDefaultAsync(x => x.Id == request.ForumId, cancellationToken);

            if (entity is null)
                throw new NotFoundException($"Forum with id {request.ForumId} was not found");

            var entityIsForum = entity.Components!.OfType<ForumComponent>().Any();

            if (entityIsForum is false)
                throw new DomainValidationException(
                    $"Entity with id {request.ForumId} is not a forum"
                );

            var message = new Message
            {
                ForumId = request.ForumId,
                Content = request.Content,
                AuthorId = domainClaims.Id,
                CreatedAt = now,
            };

            _dbContext.Add(message);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response { Message = message };
        }
    }
}
