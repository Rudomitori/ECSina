using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using ECSina.App.ApiModel;
using ECSina.App.Setup.Auth;
using ECSina.Core.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ECSina.App.Features.Auth;

[ApiController]
[Route("[controller]")]
public sealed class AuthController : ControllerBase
{
    #region Constructor and dependencies

    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly JwtTokenOptions _jwtTokenOptions;

    public AuthController(
        IMediator mediator,
        IMapper mapper,
        IOptions<JwtTokenOptions> jwtTokenOptions
    )
    {
        _mediator = mediator;
        _mapper = mapper;
        _jwtTokenOptions = jwtTokenOptions.Value;
    }

    #endregion

    public sealed class LoginRequestDto
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
    }

    public sealed class LoginResponseDto
    {
        public required ApiDataEntity Entity { get; set; }
        public required string Jwt { get; set; }
    }

    [HttpPost("Login")]
    public async Task<LoginResponseDto> Login(LoginRequestDto dto)
    {
        var response = await _mediator.Send(
            new Authenticate { Login = dto.Login, Password = dto.Password }
        );

        var jwt = new JwtSecurityToken(
            issuer: _jwtTokenOptions.Issuer,
            audience: _jwtTokenOptions.Audience,
            claims: response.Identity.Claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(4)),
            signingCredentials: new SigningCredentials(
                _jwtTokenOptions.SymmetricSecurityKey,
                SecurityAlgorithms.HmacSha256
            )
        );

        return new LoginResponseDto
        {
            Entity = _mapper.Map<ApiDataEntity>(response.Entity),
            Jwt = new JwtSecurityTokenHandler().WriteToken(jwt),
        };
    }

    public sealed class RegisterRequestDto
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
    }

    public sealed class RegisterResponseDto
    {
        public required ApiDataEntity Entity { get; set; }
        public required string Jwt { get; set; }
    }

    [HttpPost("Register")]
    public async Task<RegisterResponseDto> Register(RegisterRequestDto dto)
    {
        var response = await _mediator.Send(
            new Register { Login = dto.Login, Password = dto.Password }
        );

        var jwt = new JwtSecurityToken(
            issuer: _jwtTokenOptions.Issuer,
            audience: _jwtTokenOptions.Audience,
            claims: response.Identity.Claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(4)),
            signingCredentials: new SigningCredentials(
                _jwtTokenOptions.SymmetricSecurityKey,
                SecurityAlgorithms.HmacSha256
            )
        );

        return new RegisterResponseDto
        {
            Entity = _mapper.Map<ApiDataEntity>(response.Entity),
            Jwt = new JwtSecurityTokenHandler().WriteToken(jwt),
        };
    }
}
