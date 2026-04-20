using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.UpdateGroup;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Groups.Commands.Steps;

public class AuthorizeAndLoadGroupStep(AppDbContext db) : BaseStep<UpdateGroupContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(UpdateGroupContext context, CancellationToken ct = default)
    {
        var group = await db.AssetGroups.FirstOrDefaultAsync(
            g => g.Id == context.GroupId && g.AccountId == context.AccountId, ct);

        if (group is null)
            return Error.NotFound("Group.NotFound", "Asset group not found.");

        context.Group = group;
        return await NextAsync(context, ct);
    }
}
