using System.Text;
using ECSina.Common.Core.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECSina.App.Setup.Auth;

public class JwtTokenOptions : IPositionedOptions
{
    public static string Position => "Jwt";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Key { get; set; }
    public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.UTF8.GetBytes(Key));
}
