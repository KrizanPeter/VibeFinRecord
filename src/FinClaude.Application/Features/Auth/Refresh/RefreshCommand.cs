using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Auth.DTOs;

namespace FinClaude.Application.Features.Auth.Refresh;

public record RefreshCommand(string UserId, string RefreshToken) : ICommand<RefreshResponse>;
