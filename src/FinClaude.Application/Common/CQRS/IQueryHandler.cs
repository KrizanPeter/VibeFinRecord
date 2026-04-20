using ErrorOr;

namespace FinClaude.Application.Common.CQRS;

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<ErrorOr<TResult>> HandleAsync(TQuery query, CancellationToken ct = default);
}
