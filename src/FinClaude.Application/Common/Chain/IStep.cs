using ErrorOr;

namespace FinClaude.Application.Common.Chain;

public interface IStep<TContext>
{
    Task<ErrorOr<Success>> ExecuteAsync(TContext context, CancellationToken ct = default);
}
