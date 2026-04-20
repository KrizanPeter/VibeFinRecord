namespace FinClaude.Application.Features.Assets.DTOs;

public record AssetResponse(Guid Id, string Name, string? Institution, DateTime CreatedAt);
