using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using ECSina.Common.Core.Extensions;
using ECSina.Db.Entities;
using ECSina.Db.Entities.Auth;

namespace ECSina.Core.Features.Auth;

public sealed class DomainClaims : IEnumerable<Claim>
{
    public List<Claim> Claims { get; set; } = new();

    public Guid? Id
    {
        get => GetClaimValue().TryParseTo(out Guid id) ? id : null;
        set => SetClaimValue(value.ToString());
    }

    public string? Name
    {
        get => GetClaimValue();
        set => SetClaimValue(value);
    }

    public string? Login
    {
        get => GetClaimValue();
        set => SetClaimValue(value);
    }

    public bool IsAdmin
    {
        get => GetClaimValue() is "True";
        set => SetClaimValue(value.ToString());
    }

    public static DomainClaims From(DataEntity entity)
    {
        var userComponent = entity.Components?.OfType<UserComponent>().FirstOrDefault();
        var rolesComponent = entity.Components?.OfType<RolesComponent>().FirstOrDefault();

        return new DomainClaims
        {
            Id = entity.Id,
            Name = entity.Name,
            Login = userComponent?.Login,
            IsAdmin = rolesComponent is { IsAdmin: true }
        };
    }

    private string? GetClaimValue([CallerMemberName] string claimType = "") =>
        Claims.Find(x => x.Type == claimType)?.Value;

    private void SetClaimValue(string? value, [CallerMemberName] string claimType = "")
    {
        var index = Claims.FindIndex(x => x.Type == claimType);
        if (string.IsNullOrWhiteSpace(value))
        {
            if (index != -1)
                Claims.RemoveAt(index);
            return;
        }

        var claim = new Claim(claimType, value);

        if (index != -1)
            Claims[index] = claim;
        else
            Claims.Add(claim);
    }

    public IEnumerator<Claim> GetEnumerator() => Claims.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public static class DomainClaimsExtensions
{
    public static DomainClaims GetDomainClaims(this ClaimsPrincipal principal) =>
        new() { Claims = principal.Claims.ToList() };
}
