using FinClaude.Application.Common.CQRS;

namespace FinClaude.Application.Features.Groups.Commands.RemoveGroupMember;

public record RemoveGroupMemberCommand(Guid AccountId, Guid GroupId, Guid AssetId) : ICommand;
