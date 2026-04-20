using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Groups.DTOs;

namespace FinClaude.Application.Features.Groups.Commands.UpdateGroup;

public record UpdateGroupCommand(Guid AccountId, Guid GroupId, string Name) : ICommand<AssetGroupResponse>;
