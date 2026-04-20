using FinClaude.Api.Common.Errors;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Auth.Login;
using FinClaude.Application.Features.Auth.DTOs;
using FinClaude.Application.Features.Auth.Refresh;
using FinClaude.Application.Features.Auth.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinClaude.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
[AllowAnonymous]
public class AuthController(
    ICommandHandler<RegisterCommand, AuthResponse> registerHandler,
    ICommandHandler<LoginCommand, AuthResponse> loginHandler,
    ICommandHandler<RefreshCommand, RefreshResponse> refreshHandler) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await registerHandler.HandleAsync(new RegisterCommand(request.Email, request.Password), ct);
        return result.ToActionResult(r => CreatedAtAction(nameof(Register), r));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await loginHandler.HandleAsync(new LoginCommand(request.Email, request.Password), ct);
        return result.ToActionResult(Ok);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var result = await refreshHandler.HandleAsync(new RefreshCommand(request.UserId, request.RefreshToken), ct);
        return result.ToActionResult(Ok);
    }
}

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string UserId, string RefreshToken);
