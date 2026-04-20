using ErrorOr;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Auth.DTOs;

namespace FinClaude.Application.Features.Auth.Register;

public class RegisterCommandHandler(
    IChainProvider<RegisterContext> chainProvider,
    IUnitOfWork uow) : ICommandHandler<RegisterCommand, AuthResponse>
{
    public async Task<ErrorOr<AuthResponse>> HandleAsync(RegisterCommand command, CancellationToken ct = default)
    {
        var context = new RegisterContext { Email = command.Email, Password = command.Password };
        var chain = chainProvider.GetChain();

        await uow.BeginAsync(ct);
        var result = await chain.ExecuteAsync(context, ct);

        if (result.IsError)
        {
            await uow.RollbackAsync(ct);
            return result.Errors;
        }

        await uow.CommitAsync(ct);
        return new AuthResponse(context.AccessToken, context.RefreshToken, context.AccountId);
    }
}
