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
using FinClaude.Application.Features.Assets.Commands.CreateAsset;
using FinClaude.Application.Features.Assets.Commands.UpdateAsset;
using FinClaude.Application.Features.Assets.Commands.DeleteAsset;
using FinClaude.Application.Features.Assets.DTOs;
using FinClaude.Application.Features.Assets.Queries.ListAssets;
using FinClaude.Application.Features.Assets.Queries.GetAsset;
using FinClaude.Infrastructure.Features.Assets.Commands;
using FinClaude.Infrastructure.Features.Assets.Commands.Steps;
using FinClaude.Infrastructure.Features.Assets.Queries;
using FinClaude.Application.Features.Groups.Commands.CreateGroup;
using FinClaude.Application.Features.Groups.Commands.UpdateGroup;
using FinClaude.Application.Features.Groups.Commands.DeleteGroup;
using FinClaude.Application.Features.Groups.Commands.AddGroupMember;
using FinClaude.Application.Features.Groups.Commands.RemoveGroupMember;
using FinClaude.Application.Features.Groups.DTOs;
using FinClaude.Application.Features.Groups.Queries.ListGroups;
using FinClaude.Application.Features.Groups.Queries.GetGroup;
using FinClaude.Infrastructure.Features.Groups.Commands;
using FinClaude.Infrastructure.Features.Groups.Commands.Steps;
using FinClaude.Infrastructure.Features.Groups.Queries;
using FinClaude.Application.Common.DTOs;
using FinClaude.Application.Features.Snapshots.Commands.SubmitSnapshot;
using FinClaude.Application.Features.Snapshots.DTOs;
using FinClaude.Application.Features.Snapshots.Queries.GetSnapshot;
using FinClaude.Application.Features.Snapshots.Queries.GetSnapshotStatus;
using FinClaude.Application.Features.Snapshots.Queries.ListSnapshots;
using FinClaude.Infrastructure.Features.Auth.Refresh;
using FinClaude.Infrastructure.Features.Auth.Refresh.Steps;
using FinClaude.Infrastructure.Features.Snapshots.Commands;
using FinClaude.Infrastructure.Features.Snapshots.Commands.Steps;
using FinClaude.Infrastructure.Features.Snapshots.Queries;
using FinClaude.Application.Features.Goals.Commands.CreateGoal;
using FinClaude.Application.Features.Goals.Commands.UpdateGoal;
using FinClaude.Application.Features.Goals.Commands.DeleteGoal;
using FinClaude.Application.Features.Goals.DTOs;
using FinClaude.Application.Features.Goals.Queries.ListGoals;
using FinClaude.Application.Features.Goals.Queries.GetGoal;
using FinClaude.Infrastructure.Features.Goals.Commands;
using FinClaude.Infrastructure.Features.Goals.Commands.Steps;
using FinClaude.Infrastructure.Features.Goals.Queries;
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

        // Assets
        services.AddTransient<IQueryHandler<ListAssetsQuery, List<AssetResponse>>, ListAssetsQueryHandler>();
        services.AddTransient<IQueryHandler<GetAssetQuery, AssetResponse>, GetAssetQueryHandler>();
        services.AddTransient<ValidateCreateAssetStep>();
        services.AddTransient<PersistCreateAssetStep>();
        services.AddTransient<IChainProvider<CreateAssetContext>, CreateAssetChainProvider>();
        services.AddTransient<ICommandHandler<CreateAssetCommand, AssetResponse>, CreateAssetCommandHandler>();
        services.AddTransient<ValidateUpdateAssetStep>();
        services.AddTransient<AuthorizeAndLoadAssetStep>();
        services.AddTransient<PersistUpdateAssetStep>();
        services.AddTransient<IChainProvider<UpdateAssetContext>, UpdateAssetChainProvider>();
        services.AddTransient<ICommandHandler<UpdateAssetCommand, AssetResponse>, UpdateAssetCommandHandler>();
        services.AddTransient<AuthorizeAndDeleteAssetStep>();
        services.AddTransient<IChainProvider<DeleteAssetContext>, DeleteAssetChainProvider>();
        services.AddTransient<ICommandHandler<DeleteAssetCommand>, DeleteAssetCommandHandler>();

        // Groups
        services.AddTransient<IQueryHandler<ListGroupsQuery, List<AssetGroupResponse>>, ListGroupsQueryHandler>();
        services.AddTransient<IQueryHandler<GetGroupQuery, AssetGroupDetailResponse>, GetGroupQueryHandler>();
        services.AddTransient<ValidateCreateGroupStep>();
        services.AddTransient<PersistCreateGroupStep>();
        services.AddTransient<IChainProvider<CreateGroupContext>, CreateGroupChainProvider>();
        services.AddTransient<ICommandHandler<CreateGroupCommand, AssetGroupResponse>, CreateGroupCommandHandler>();
        services.AddTransient<ValidateUpdateGroupStep>();
        services.AddTransient<AuthorizeAndLoadGroupStep>();
        services.AddTransient<PersistUpdateGroupStep>();
        services.AddTransient<IChainProvider<UpdateGroupContext>, UpdateGroupChainProvider>();
        services.AddTransient<ICommandHandler<UpdateGroupCommand, AssetGroupResponse>, UpdateGroupCommandHandler>();
        services.AddTransient<AuthorizeAndDeleteGroupStep>();
        services.AddTransient<IChainProvider<DeleteGroupContext>, DeleteGroupChainProvider>();
        services.AddTransient<ICommandHandler<DeleteGroupCommand>, DeleteGroupCommandHandler>();
        services.AddTransient<ValidateAndPersistAddMemberStep>();
        services.AddTransient<IChainProvider<AddGroupMemberContext>, AddGroupMemberChainProvider>();
        services.AddTransient<ICommandHandler<AddGroupMemberCommand>, AddGroupMemberCommandHandler>();
        services.AddTransient<AuthorizeAndRemoveMemberStep>();
        services.AddTransient<IChainProvider<RemoveGroupMemberContext>, RemoveGroupMemberChainProvider>();
        services.AddTransient<ICommandHandler<RemoveGroupMemberCommand>, RemoveGroupMemberCommandHandler>();

        // Snapshots
        services.AddTransient<IQueryHandler<GetSnapshotStatusQuery, SnapshotStatusResponse>, GetSnapshotStatusQueryHandler>();
        services.AddTransient<IQueryHandler<ListSnapshotsQuery, PagedResponse<SnapshotSummaryResponse>>, ListSnapshotsQueryHandler>();
        services.AddTransient<IQueryHandler<GetSnapshotQuery, SnapshotDetailResponse>, GetSnapshotQueryHandler>();
        services.AddTransient<ValidateSnapshotSubmitStep>();
        services.AddTransient<PersistSnapshotStep>();
        services.AddTransient<IChainProvider<SubmitSnapshotContext>, SubmitSnapshotChainProvider>();
        services.AddTransient<ICommandHandler<SubmitSnapshotCommand, SnapshotDetailResponse>, SubmitSnapshotCommandHandler>();

        // Goals
        services.AddTransient<IQueryHandler<ListGoalsQuery, List<GoalResponse>>, ListGoalsQueryHandler>();
        services.AddTransient<IQueryHandler<GetGoalQuery, GoalResponse>, GetGoalQueryHandler>();
        services.AddTransient<ValidateCreateGoalStep>();
        services.AddTransient<AuthorizeLinkedEntityForCreateStep>();
        services.AddTransient<PersistCreateGoalStep>();
        services.AddTransient<IChainProvider<CreateGoalContext>, CreateGoalChainProvider>();
        services.AddTransient<ICommandHandler<CreateGoalCommand, GoalResponse>, CreateGoalCommandHandler>();
        services.AddTransient<ValidateUpdateGoalStep>();
        services.AddTransient<AuthorizeAndLoadGoalStep>();
        services.AddTransient<AuthorizeLinkedEntityForUpdateStep>();
        services.AddTransient<PersistUpdateGoalStep>();
        services.AddTransient<IChainProvider<UpdateGoalContext>, UpdateGoalChainProvider>();
        services.AddTransient<ICommandHandler<UpdateGoalCommand, GoalResponse>, UpdateGoalCommandHandler>();
        services.AddTransient<AuthorizeAndDeleteGoalStep>();
        services.AddTransient<IChainProvider<DeleteGoalContext>, DeleteGoalChainProvider>();
        services.AddTransient<ICommandHandler<DeleteGoalCommand>, DeleteGoalCommandHandler>();

        return services;
    }
}
