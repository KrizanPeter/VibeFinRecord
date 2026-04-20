using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.RemoveGroupMember;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Groups.Commands.Steps;

public class AuthorizeAndRemoveMemberStep(AppDbContext db) : BaseStep<RemoveGroupMemberContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(RemoveGroupMemberContext context, CancellationToken ct = default)
    {
        var groupExists = await db.AssetGroups.AnyAsync(
            g => g.Id == context.GroupId && g.AccountId == context.AccountId, ct);

        if (!groupExists)
            return Error.NotFound("Group.NotFound", "Asset group not found.");

        var membership = await db.AssetGroupMemberships.FirstOrDefaultAsync(
            m => m.GroupId == context.GroupId && m.AssetId == context.AssetId, ct);

        if (membership is null)
            return Error.NotFound("Group.MemberNotFound", "Asset is not a member of this group.");

        db.AssetGroupMemberships.Remove(membership);

        return await NextAsync(context, ct);
    }
}
