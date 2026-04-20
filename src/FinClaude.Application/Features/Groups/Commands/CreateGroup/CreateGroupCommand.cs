using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Groups.DTOs;

namespace FinClaude.Application.Features.Groups.Commands.CreateGroup;

public record CreateGroupCommand(Guid AccountId, string Name) : ICommand<AssetGroupResponse>;
