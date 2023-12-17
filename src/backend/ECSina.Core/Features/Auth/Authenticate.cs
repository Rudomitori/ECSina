using System.Security.Claims;
using ECSina.Common.Core.Exceptions;
using ECSina.Core.Base;
using ECSina.Db;
using ECSina.Db.Entities;
using ECSina.Db.Entities.Auth;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECSina.Core.Features.Auth;

public sealed class Authenticate : CommandBase<Authenticate.Response>
{
    public required string Login { get; set; }
    public required string Password { get; set; }

    public sealed class Response
    {
        public required DataEntity Entity { get; set; }
        public required ClaimsIdentity Identity { get; set; }
    }

    public sealed class Validator : AbstractValidator<Authenticate>
    {
        public Validator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Authenticate, Response>
    {
        #region Constructor and dependencies

        private readonly AppDbContext _dbContext;
        private readonly PasswordHasher _passwordHasher;

        public Handler(AppDbContext dbContext, PasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        #endregion

        public async Task<Response> Handle(
            Authenticate request,
            CancellationToken cancellationToken
        )
        {
            var login = StringNormalizer.NormalizeLogin(request.Login);
            var normalizedLogin = StringNormalizer.NormalizeLoginToIndex(login);

            var entity = await _dbContext
                .Set<DataEntity>()
                .Include(x => x.Components)
                .FirstOrDefaultAsync(
                    x =>
                        x.Components!.OfType<UserComponent>()
                            .Any(y => y.NormalizedLogin == normalizedLogin),
                    cancellationToken
                );

            var userComponent = entity?.Components!.OfType<UserComponent>().FirstOrDefault();

            var authenticated =
                userComponent is { }
                && _passwordHasher.VerifyHashedPassword(
                    entity!,
                    userComponent.PasswordHash,
                    request.Password
                );

            if (!authenticated)
                throw new NotFoundException(
                    "A user with a provided login and a password was not found"
                );

            return new Response
            {
                Entity = entity!,
                Identity = new ClaimsIdentity(DomainClaims.From(entity!))
            };
        }
    }
}
