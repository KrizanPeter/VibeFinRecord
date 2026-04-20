using ErrorOr;

namespace FinClaude.Application.Common.Chain;

public abstract class BaseStep<TContext> : IStep<TContext>
{
    private IStep<TContext>? _next;

    public BaseStep<TContext> SetNext(BaseStep<TContext> next)
    {
        _next = next;
        return next;
    }

    protected Task<ErrorOr<Success>> NextAsync(TContext context, CancellationToken ct) =>
        _next is not null
            ? _next.ExecuteAsync(context, ct)
            : Task.FromResult<ErrorOr<Success>>(Result.Success);

    public abstract Task<ErrorOr<Success>> ExecuteAsync(TContext context, CancellationToken ct = default);
}
