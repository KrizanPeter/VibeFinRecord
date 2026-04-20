using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Auth.DTOs;

namespace FinClaude.Application.Features.Auth.Refresh;

public class RefreshCommandHandler(
    IChainProvider<RefreshContext> chainProvider) : ICommandHandler<RefreshCommand, RefreshResponse>
{
    public async Task<ErrorOr<RefreshResponse>> HandleAsync(RefreshCommand command, CancellationToken ct = default)
    {
        var context = new RefreshContext { UserId = command.UserId, CurrentRefreshToken = command.RefreshToken };
        var chain = chainProvider.GetChain();

        var result = await chain.ExecuteAsync(context, ct);
        if (result.IsError)
            return result.Errors;

        return new RefreshResponse(context.AccessToken, context.NewRefreshToken);
    }
}
