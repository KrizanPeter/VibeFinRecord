namespace FinClaude.Application.Features.Groups.DTOs;

public record AssetGroupResponse(Guid Id, string Name, DateTime CreatedAt);

public record AssetGroupDetailResponse(Guid Id, string Name, DateTime CreatedAt, List<Guid> MemberAssetIds);
