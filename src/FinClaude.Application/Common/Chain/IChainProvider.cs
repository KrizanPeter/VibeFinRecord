namespace FinClaude.Application.Common.Chain;

public interface IChainProvider<TContext>
{
    IStep<TContext> GetChain();
}
