using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.AddGroupMember;
using FinClaude.Domain.Entities;
using FinClaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Infrastructure.Features.Groups.Commands.Steps;

public class ValidateAndPersistAddMemberStep(AppDbContext db) : BaseStep<AddGroupMemberContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(AddGroupMemberContext context, CancellationToken ct = default)
    {
        var groupExists = await db.AssetGroups.AnyAsync(
            g => g.Id == context.GroupId && g.AccountId == context.AccountId, ct);

        if (!groupExists)
            return Error.NotFound("Group.NotFound", "Asset group not found.");

        var assetExists = await db.Assets.AnyAsync(
            a => a.Id == context.AssetId && a.AccountId == context.AccountId, ct);

        if (!assetExists)
            return Error.NotFound("Asset.NotFound", "Asset not found.");

        var alreadyMember = await db.AssetGroupMemberships.AnyAsync(
            m => m.GroupId == context.GroupId && m.AssetId == context.AssetId, ct);

        if (alreadyMember)
            return Error.Conflict("Group.AlreadyMember", "Asset is already a member of this group.");

        db.AssetGroupMemberships.Add(new AssetGroupMembership { GroupId = context.GroupId, AssetId = context.AssetId });

        return await NextAsync(context, ct);
    }
}
