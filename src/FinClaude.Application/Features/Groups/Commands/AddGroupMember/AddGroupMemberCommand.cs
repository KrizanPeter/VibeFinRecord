using FinClaude.Application.Common.CQRS;

namespace FinClaude.Application.Features.Groups.Commands.AddGroupMember;

public record AddGroupMemberCommand(Guid AccountId, Guid GroupId, Guid AssetId) : ICommand;
