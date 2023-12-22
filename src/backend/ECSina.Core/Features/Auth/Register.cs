using System.Security.Claims;
using ECSina.Common.Core.Clock;
using ECSina.Common.Core.Exceptions;
using ECSina.Core.Base;
using ECSina.Db;
using ECSina.Db.Entities;
using ECSina.Db.Entities.Auth;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECSina.Core.Features.Auth;

public sealed class Register : CommandBase<Register.Response>
{
    public required string Login { get; set; }
    public required string Password { get; set; }

    public sealed class Response
    {
        public required DataEntity Entity { get; set; }
        public required ClaimsIdentity Identity { get; set; }
    }

    public sealed class Validator : AbstractValidator<Register>
    {
        public Validator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Register, Response>
    {
        #region Constructor and dependencies

        private readonly AppDbContext _dbContext;
        private readonly IClock _clock;
        private readonly PasswordHasher _passwordHasher;
        private readonly PasswordValidator _passwordValidator;
        private readonly ClaimsPrincipal _principal;

        public Handler(
            AppDbContext dbContext,
            IClock clock,
            PasswordHasher passwordHasher,
            PasswordValidator passwordValidator,
            ClaimsPrincipal principal
        )
        {
            _dbContext = dbContext;
            _clock = clock;
            _passwordHasher = passwordHasher;
            _passwordValidator = passwordValidator;
            _principal = principal;
        }

        #endregion

        public async Task<Response> Handle(Register request, CancellationToken cancellationToken)
        {
            var now = _clock.UtcNow;
            var domainClaims = _principal.GetDomainClaims();

            var errorType = _passwordValidator.Validate(request.Password);
            if (errorType is { })
                throw new DomainValidationException(errorType.Value.ToString());

            var login = StringNormalizer.NormalizeLogin(request.Login);
            var normalizedLogin = StringNormalizer.NormalizeLoginToIndex(login);

            var loginExists = await _dbContext
                .Set<DataEntity>()
                .Include(x => x.Components)
                .AnyAsync(
                    x =>
                        x.Components!.OfType<UserComponent>()
                            .Any(y => y.NormalizedLogin == normalizedLogin),
                    cancellationToken
                );

            if (loginExists)
                throw new DomainValidationException("A user with this login already exists");

            var entity = new DataEntity
            {
                Name = login,
                CreatedAt = now,
                CreatedById = domainClaims.Id,
                Components = new List<DataComponent>()
            };

            entity.Components.Add(
                new PasswordComponent
                {
                    EntityId = entity.Id,
                    PasswordHash = _passwordHasher.GenerateHash(request.Password),
                    CreatedAt = now,
                    CreatedById = domainClaims.Id,
                }
            );

            entity.Components.Add(
                new UserComponent
                {
                    EntityId = entity.Id,
                    Login = login,
                    NormalizedLogin = normalizedLogin,
                    CreatedAt = now,
                    CreatedById = domainClaims.Id,
                }
            );

            _dbContext.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response
            {
                Entity = entity,
                Identity = new ClaimsIdentity(DomainClaims.From(entity))
            };
        }
    }
}
