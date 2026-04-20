using ErrorOr;

namespace FinClaude.Application.Common.CQRS;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task<ErrorOr<Success>> HandleAsync(TCommand command, CancellationToken ct = default);
}

public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<ErrorOr<TResult>> HandleAsync(TCommand command, CancellationToken ct = default);
}
