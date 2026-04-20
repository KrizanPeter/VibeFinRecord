using FinClaude.Application.Common.CQRS;

namespace FinClaude.Application.Features.Groups.Commands.DeleteGroup;

public record DeleteGroupCommand(Guid AccountId, Guid GroupId) : ICommand;
