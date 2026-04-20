using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Goals.Commands.CreateGoal;

namespace FinClaude.Infrastructure.Features.Goals.Commands.Steps;

public class ValidateCreateGoalStep : BaseStep<CreateGoalContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateGoalContext context, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(context.Name))
            return Error.Validation("Goal.NameRequired", "Goal name is required.");

        if (context.Name.Length > 200)
            return Error.Validation("Goal.NameTooLong", "Goal name cannot exceed 200 characters.");

        if (context.TargetValue <= 0)
            return Error.Validation("Goal.TargetValueInvalid", "Target value must be greater than zero.");

        if (context.AssetId.HasValue == context.GroupId.HasValue)
            return Error.Validation("Goal.LinkedEntityInvalid", "A goal must link to exactly one Asset or AssetGroup.");

        return await NextAsync(context, ct);
    }
}
