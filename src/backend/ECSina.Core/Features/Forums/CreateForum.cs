using System.Security.Claims;
using ECSina.Common.Core.Clock;
using ECSina.Common.Core.Exceptions;
using ECSina.Core.Base;
using ECSina.Core.Features.Auth;
using ECSina.Db;
using ECSina.Db.Entities;
using ECSina.Db.Entities.Forums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECSina.Core.Features.Forums;

public sealed class CreateForum : CommandBase<CreateForum.Response>
{
    public required string Name { get; set; }
    public Guid? ParentId { get; set; }

    public sealed class Response
    {
        public required DataEntity Entity { get; set; }
    }

    public sealed class Validator : AbstractValidator<CreateForum>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.ParentId).NotEqual(Guid.Empty);
        }
    }

    public sealed class Handler : IRequestHandler<CreateForum, Response>
    {
        #region Constructor and dependencies

        private readonly AppDbContext _dbContext;
        private readonly IClock _clock;
        private readonly ClaimsPrincipal _principal;

        public Handler(AppDbContext dbContext, IClock clock, ClaimsPrincipal principal)
        {
            _dbContext = dbContext;
            _clock = clock;
            _principal = principal;
        }

        #endregion

        public async Task<Response> Handle(CreateForum request, CancellationToken cancellationToken)
        {
            if (_principal.Identity is not { IsAuthenticated: true })
                throw new ForbiddenException("Only an authenticated user can create forum");

            var now = _clock.UtcNow;
            var domainClaims = _principal.GetDomainClaims();

            var entity = new DataEntity
            {
                Name = StringNormalizer.NormalizeEntityName(request.Name),
                CreatedAt = now,
                Components = new List<DataComponent>(),
                CreatedById = domainClaims.Id
            };

            entity.Components.Add(
                new ForumComponent
                {
                    EntityId = entity.Id,
                    CreatedAt = now,
                    CreatedById = domainClaims.Id
                }
            );

            if (request.ParentId is { } parentId)
            {
                var parentExists = await _dbContext
                    .Set<DataEntity>()
                    .AnyAsync(x => x.Id == parentId, cancellationToken);

                if (!parentExists)
                    throw new NotFoundException($"An entity with id {parentId} was not found");

                entity.Components.Add(
                    new HierarchyComponent
                    {
                        EntityId = entity.Id,
                        ParentId = parentId,
                        CreatedAt = now,
                        CreatedById = domainClaims.Id
                    }
                );
            }

            _dbContext.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response { Entity = entity };
        }
    }
}
