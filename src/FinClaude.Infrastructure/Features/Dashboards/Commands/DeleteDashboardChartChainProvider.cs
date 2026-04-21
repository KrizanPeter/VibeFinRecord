using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Dashboards.Commands.DeleteDashboardChart;
using FinClaude.Infrastructure.Features.Dashboards.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Dashboards.Commands;

public class DeleteDashboardChartChainProvider(
    AuthorizeAndDeleteDashboardChartStep authorizeAndDelete) : IChainProvider<DeleteDashboardChartContext>
{
    public IStep<DeleteDashboardChartContext> GetChain()
    {
        return authorizeAndDelete;
    }
}
