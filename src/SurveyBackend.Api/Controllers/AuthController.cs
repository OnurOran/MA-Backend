using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SurveyBackend.Api.Contracts;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Modules.Auth.Commands.Admin;
using SurveyBackend.Application.Modules.Auth.Commands.Login;
using SurveyBackend.Application.Modules.Auth.Commands.Logout;
using SurveyBackend.Application.Modules.Auth.Commands.Refresh;
using SurveyBackend.Application.Modules.Auth.DTOs;
using SurveyBackend.Application.Modules.Auth.Mappings;
using SurveyBackend.Infrastructure.Configurations;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAppMediator _mediator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;
    private readonly IValidator<RefreshTokenRequest> _refreshTokenRequestValidator;
    private readonly IValidator<UpdateLocalAdminPasswordCommand> _updatePasswordValidator;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        IAppMediator mediator,
        IValidator<LoginRequest> loginRequestValidator,
        IValidator<RefreshTokenRequest> refreshTokenRequestValidator,
        IValidator<UpdateLocalAdminPasswordCommand> updatePasswordValidator,
        IOptions<JwtSettings> jwtOptions)
    {
        _mediator = mediator;
        _loginRequestValidator = loginRequestValidator;
        _refreshTokenRequestValidator = refreshTokenRequestValidator;
        _updatePasswordValidator = updatePasswordValidator;
        _jwtSettings = jwtOptions.Value;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _loginRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            AddErrorsToModelState(validationResult);
            return ValidationProblem(ModelState);
        }

        var command = AuthMapper.ToLoginCommand(request);
        var response = await _mediator.SendAsync<LoginCommand, AuthTokensDto>(command, cancellationToken);
        SetAuthCookies(response);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest? request, CancellationToken cancellationToken)
    {
        if (request is not null && !string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var validationResult = await _refreshTokenRequestValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                AddErrorsToModelState(validationResult);
                return ValidationProblem(ModelState);
            }
        }

        var refreshToken = ResolveRefreshToken(request);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized("Refresh token bulunamadı.");
        }

        var command = new RefreshTokenCommand(refreshToken);
        var response = await _mediator.SendAsync<RefreshTokenCommand, AuthTokensDto>(command, cancellationToken);
        SetAuthCookies(response);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest? request, CancellationToken cancellationToken)
    {
        var refreshToken = ResolveRefreshToken(request);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            ClearAuthCookies();
            return NoContent();
        }

        var command = new LogoutCommand(refreshToken);
        await _mediator.SendAsync<LogoutCommand, bool>(command, cancellationToken);
        ClearAuthCookies();
        return NoContent();
    }

    [Authorize]
    [HttpGet("test")]
    public IActionResult Test()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(new {
            isAuthenticated = User.Identity?.IsAuthenticated,
            claims = claims
        });
    }

    [Authorize]
    [HttpPost("admin/password")]
    public async Task<IActionResult> UpdateAdminPassword([FromBody] UpdateAdminPasswordRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest("Geçersiz istek.");
        }

        var command = new UpdateLocalAdminPasswordCommand(request.NewPassword);
        var validationResult = await _updatePasswordValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            AddErrorsToModelState(validationResult);
            return ValidationProblem(ModelState);
        }

        await _mediator.SendAsync<UpdateLocalAdminPasswordCommand, bool>(command, cancellationToken);
        return NoContent();
    }

    private void AddErrorsToModelState(FluentValidation.Results.ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }

    private string? ResolveRefreshToken(RefreshTokenRequest? request) =>
        ResolveRefreshToken(request?.RefreshToken);

    private string? ResolveRefreshToken(LogoutRequest? request) =>
        ResolveRefreshToken(request?.RefreshToken);

    private string? ResolveRefreshToken(string? requestToken)
    {
        if (!string.IsNullOrWhiteSpace(requestToken))
        {
            return requestToken;
        }

        Request.Cookies.TryGetValue(_jwtSettings.RefreshTokenCookieName, out var refreshCookie);
        return refreshCookie;
    }

    private void SetAuthCookies(AuthTokensDto tokens)
    {
        if (!_jwtSettings.UseCookies)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var sameSite = ResolveSameSiteMode(_jwtSettings.CookieSameSite);

        var accessCookie = new CookieOptions
        {
            HttpOnly = true,
            Secure = _jwtSettings.SecureCookies,
            SameSite = sameSite,
            Path = _jwtSettings.AccessTokenCookiePath,
            Expires = now.AddMinutes(_jwtSettings.AccessTokenMinutes),
            MaxAge = TimeSpan.FromMinutes(_jwtSettings.AccessTokenMinutes),
            IsEssential = true
        };

        var refreshCookie = new CookieOptions
        {
            HttpOnly = true,
            Secure = _jwtSettings.SecureCookies,
            SameSite = sameSite,
            Path = _jwtSettings.RefreshTokenCookiePath,
            Expires = now.AddDays(_jwtSettings.RefreshTokenDays),
            MaxAge = TimeSpan.FromDays(_jwtSettings.RefreshTokenDays),
            IsEssential = true
        };

        Response.Cookies.Append(_jwtSettings.AccessTokenCookieName, tokens.AccessToken, accessCookie);
        Response.Cookies.Append(_jwtSettings.RefreshTokenCookieName, tokens.RefreshToken, refreshCookie);
        Response.Headers.CacheControl = "no-store, no-cache";
        Response.Headers.Pragma = "no-cache";
    }

    private void ClearAuthCookies()
    {
        if (!_jwtSettings.UseCookies)
        {
            return;
        }

        var sameSite = ResolveSameSiteMode(_jwtSettings.CookieSameSite);

        Response.Cookies.Delete(_jwtSettings.AccessTokenCookieName, new CookieOptions
        {
            Secure = _jwtSettings.SecureCookies,
            SameSite = sameSite,
            Path = _jwtSettings.AccessTokenCookiePath
        });

        Response.Cookies.Delete(_jwtSettings.RefreshTokenCookieName, new CookieOptions
        {
            Secure = _jwtSettings.SecureCookies,
            SameSite = sameSite,
            Path = _jwtSettings.RefreshTokenCookiePath
        });
    }

    private static SameSiteMode ResolveSameSiteMode(string? value)
    {
        if (!Enum.TryParse<SameSiteMode>(value, ignoreCase: true, out var parsed))
        {
            return SameSiteMode.None;
        }

        return parsed;
    }
}
