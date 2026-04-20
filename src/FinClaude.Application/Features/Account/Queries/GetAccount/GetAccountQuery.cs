using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Account.DTOs;

namespace FinClaude.Application.Features.Account.Queries.GetAccount;

public record GetAccountQuery(Guid AccountId) : IQuery<AccountResponse>;
