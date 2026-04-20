# TASK-05 — ASP.NET Core Identity + JWT Setup

## Description

Configure ASP.NET Core Identity and JWT authentication in `FinClaude.Infrastructure` and `FinClaude.Api`.

- `IdentityUser` setup (email/password only, no roles)
- JWT configuration: 24h access token expiry (dev), `AccountId` included as custom claim
- Refresh token storage and validation
- `ITokenService` interface (Application) + implementation (Infrastructure)
- DI registration in `Program.cs`

## Dependencies

TASK-04

## Status

Done
