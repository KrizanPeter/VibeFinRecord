using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Auth.Refresh;
using FinClaude.Infrastructure.Features.Auth.Refresh.Steps;

namespace FinClaude.Infrastructure.Features.Auth.Refresh;

public class RefreshChainProvider(ValidateAndRotateRefreshStep validateAndRotate) : IChainProvider<RefreshContext>
{
    public IStep<RefreshContext> GetChain() => validateAndRotate;
}
