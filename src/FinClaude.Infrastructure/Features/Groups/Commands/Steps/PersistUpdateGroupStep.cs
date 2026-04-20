using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.UpdateGroup;

namespace FinClaude.Infrastructure.Features.Groups.Commands.Steps;

public class PersistUpdateGroupStep : BaseStep<UpdateGroupContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateGroupContext context, CancellationToken ct = default)
    {
        context.Group!.Name = context.Name;
        return await NextAsync(context, ct);
    }
}
