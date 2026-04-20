using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Assets.Commands.CreateAsset;

namespace FinClaude.Infrastructure.Features.Assets.Commands.Steps;

public class ValidateCreateAssetStep : BaseStep<CreateAssetContext>
{
    public override async Task<ErrorOr<Success>> ExecuteAsync(CreateAssetContext context, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(context.Name))
            return Error.Validation("Asset.NameRequired", "Asset name is required.");

        if (context.Name.Length > 200)
            return Error.Validation("Asset.NameTooLong", "Asset name cannot exceed 200 characters.");

        return await NextAsync(context, ct);
    }
}
