using System.Security.Claims;
using System.Text.Json;
using FinClaude.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinClaude.Api.Common.Middleware;

public class AccountSetupGuardMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly HashSet<string> BypassPrefixes =
        ["/api/v1/auth/"];

    private static readonly (string Method, string Path) BypassAccountPut =
        ("PUT", "/api/v1/account");

    public AccountSetupGuardMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var method = context.Request.Method;

        if (IsBypassed(method, path))
        {
            await _next(context);
            return;
        }

        var user = context.User;
        if (user.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var accountId = user.FindFirstValue("account_id");
        if (accountId is null || !Guid.TryParse(accountId, out var id))
        {
            await _next(context);
            return;
        }

        var account = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt == null);

        if (account?.SnapshotStartDate is null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = 403,
                Title = "account-setup-required",
                Detail = "Complete account setup before accessing this resource.",
                Type = "account-setup-required",
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            return;
        }

        await _next(context);
    }

    private static bool IsBypassed(string method, string path)
    {
        foreach (var prefix in BypassPrefixes)
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return true;

        return string.Equals(method, BypassAccountPut.Method, StringComparison.OrdinalIgnoreCase)
            && string.Equals(path, BypassAccountPut.Path, StringComparison.OrdinalIgnoreCase);
    }
}
