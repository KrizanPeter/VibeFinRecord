using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.CreateGroup;

namespace FinClaude.Infrastructure.Features.Groups.Commands.Steps;

public class ValidateCreateGroupStep : BaseStep<CreateGroupContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateGroupContext context, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(context.Name))
            return Error.Validation("Group.NameRequired", "Group name is required.");

        if (context.Name.Length > 200)
            return Error.Validation("Group.NameTooLong", "Group name cannot exceed 200 characters.");

        return await NextAsync(context, ct);
    }
}
