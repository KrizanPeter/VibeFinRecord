using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Auth.DTOs;

namespace FinClaude.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<AuthResponse>;
