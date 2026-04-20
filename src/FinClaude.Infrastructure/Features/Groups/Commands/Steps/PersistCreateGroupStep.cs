using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Groups.Commands.CreateGroup;
using FinClaude.Domain.Entities;
using FinClaude.Infrastructure.Persistence;

namespace FinClaude.Infrastructure.Features.Groups.Commands.Steps;

public class PersistCreateGroupStep(AppDbContext db) : BaseStep<CreateGroupContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateGroupContext context, CancellationToken ct = default)
    {
        var group = new AssetGroup { AccountId = context.AccountId, Name = context.Name };
        db.AssetGroups.Add(group);
        context.CreatedGroup = group;

        return await NextAsync(context, ct);
    }
}
