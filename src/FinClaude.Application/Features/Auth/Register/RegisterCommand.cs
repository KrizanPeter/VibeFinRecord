using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Auth.DTOs;

namespace FinClaude.Application.Features.Auth.Register;

public record RegisterCommand(string Email, string Password) : ICommand<AuthResponse>;
