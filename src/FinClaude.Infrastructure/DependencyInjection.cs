using System.Text;
using FinClaude.Application.Common.Auth;
using FinClaude.Application.Common.Chain;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Common.Persistence;
using FinClaude.Application.Features.Auth;
using FinClaude.Application.Features.Auth.DTOs;
using FinClaude.Application.Features.Auth.Login;
using FinClaude.Application.Features.Account.Commands.UpdateAccount;
using FinClaude.Application.Features.Account.DTOs;
using FinClaude.Application.Features.Account.Queries.GetAccount;
using FinClaude.Application.Features.Auth.Refresh;
using FinClaude.Application.Features.Auth.Register;
using FinClaude.Infrastructure.Auth;
using FinClaude.Infrastructure.Features.Auth;
using FinClaude.Infrastructure.Features.Auth.Login;
using FinClaude.Infrastructure.Features.Auth.Login.Steps;
using FinClaude.Infrastructure.Features.Account.Commands;
using FinClaude.Infrastructure.Features.Account.Commands.Steps;
using FinClaude.Infrastructure.Features.Account.Queries;
using FinClaude.Infrastructure.Features.Auth.Refresh;
using FinClaude.Infrastructure.Features.Auth.Refresh.Steps;
using FinClaude.Infrastructure.Features.Auth.Register;
using FinClaude.Infrastructure.Features.Auth.Register.Steps;
using FinClaude.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FinClaude.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(config.GetConnectionString("DefaultConnection")));

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IIdentityService, IdentityService>();

        // Auth — Register
        services.AddTransient<CreateIdentityUserStep>();
        services.AddTransient<CreateAccountStep>();
        services.AddTransient<GenerateRegisterTokensStep>();
        services.AddTransient<IChainProvider<RegisterContext>, RegisterChainProvider>();
        services.AddTransient<ICommandHandler<RegisterCommand, AuthResponse>, RegisterCommandHandler>();

        // Auth — Login
        services.AddTransient<ValidateCredentialsStep>();
        services.AddTransient<GenerateLoginTokensStep>();
        services.AddTransient<IChainProvider<LoginContext>, LoginChainProvider>();
        services.AddTransient<ICommandHandler<LoginCommand, AuthResponse>, LoginCommandHandler>();

        // Auth — Refresh
        services.AddTransient<ValidateAndRotateRefreshStep>();
        services.AddTransient<IChainProvider<RefreshContext>, RefreshChainProvider>();
        services.AddTransient<ICommandHandler<RefreshCommand, RefreshResponse>, RefreshCommandHandler>();

        // Account
        services.AddTransient<IQueryHandler<GetAccountQuery, AccountResponse>, GetAccountQueryHandler>();
        services.AddTransient<ValidateAccountSetupStep>();
        services.AddTransient<LockIfSnapshotsExistStep>();
        services.AddTransient<PersistAccountSetupStep>();
        services.AddTransient<IChainProvider<UpdateAccountContext>, UpdateAccountChainProvider>();
        services.AddTransient<ICommandHandler<UpdateAccountCommand, AccountResponse>, UpdateAccountCommandHandler>();

        var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>()!;
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

        services.AddAuthorization();

        return services;
    }
}
