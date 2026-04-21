using System.Security.Claims;
using FinClaude.Api.Common.Errors;
using FinClaude.Application.Common.CQRS;
using FinClaude.Application.Features.Dashboards.Commands.CreateDashboardChart;
using FinClaude.Application.Features.Dashboards.Commands.DeleteDashboardChart;
using FinClaude.Application.Features.Dashboards.Commands.UpdateDashboardChart;
using FinClaude.Application.Features.Dashboards.DTOs;
using FinClaude.Application.Features.Dashboards.Queries.GetChartData;
using FinClaude.Application.Features.Dashboards.Queries.GetDashboardChart;
using FinClaude.Application.Features.Dashboards.Queries.ListDashboardCharts;
using FinClaude.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinClaude.Api.Controllers;

[ApiController]
[Route("api/v1/dashboard/charts")]
[Authorize]
public class DashboardChartsController(
    IQueryHandler<ListDashboardChartsQuery, List<DashboardChartResponse>> listHandler,
    IQueryHandler<GetDashboardChartQuery, DashboardChartResponse> getHandler,
    IQueryHandler<GetChartDataQuery, ChartDataResponse> dataHandler,
    ICommandHandler<CreateDashboardChartCommand, DashboardChartResponse> createHandler,
    ICommandHandler<UpdateDashboardChartCommand, DashboardChartResponse> updateHandler,
    ICommandHandler<DeleteDashboardChartCommand> deleteHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListCharts(CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await listHandler.HandleAsync(new ListDashboardChartsQuery(accountId.Value), ct);
        return result.ToActionResult(Ok);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetChart(Guid id, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await getHandler.HandleAsync(new GetDashboardChartQuery(accountId.Value, id), ct);
        return result.ToActionResult(Ok);
    }

    [HttpPost]
    public async Task<IActionResult> CreateChart([FromBody] CreateDashboardChartRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await createHandler.HandleAsync(
            new CreateDashboardChartCommand(accountId.Value, request.Name, request.ChartType, request.SourceType, request.AssetId, request.GroupId), ct);
        return result.ToActionResult(created => CreatedAtAction(nameof(GetChart), new { id = created.Id }, created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateChart(Guid id, [FromBody] UpdateDashboardChartRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await updateHandler.HandleAsync(
            new UpdateDashboardChartCommand(accountId.Value, id, request.Name, request.ChartType, request.SourceType, request.AssetId, request.GroupId), ct);
        return result.ToActionResult(Ok);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteChart(Guid id, CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await deleteHandler.HandleAsync(new DeleteDashboardChartCommand(accountId.Value, id), ct);
        return result.ToActionResult(_ => NoContent());
    }

    [HttpGet("{id:guid}/data")]
    public async Task<IActionResult> GetChartData(Guid id, [FromQuery] string range = "all", CancellationToken ct = default)
    {
        var accountId = GetAccountId();
        if (accountId is null) return Unauthorized();

        var result = await dataHandler.HandleAsync(new GetChartDataQuery(accountId.Value, id, range), ct);
        return result.ToActionResult(Ok);
    }

    private Guid? GetAccountId()
    {
        var claim = User.FindFirstValue("account_id");
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}

public record CreateDashboardChartRequest(string Name, ChartType ChartType, SourceType SourceType, Guid? AssetId, Guid? GroupId);
public record UpdateDashboardChartRequest(string Name, ChartType ChartType, SourceType SourceType, Guid? AssetId, Guid? GroupId);
