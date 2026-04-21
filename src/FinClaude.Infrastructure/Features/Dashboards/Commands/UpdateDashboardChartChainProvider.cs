using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Dashboards.Commands.UpdateDashboardChart;
using FinClaude.Infrastructure.Features.Dashboards.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Dashboards.Commands;

public class UpdateDashboardChartChainProvider(
    AuthorizeAndLoadDashboardChartStep authorizeAndLoad,
    ValidateUpdateDashboardChartStep validate,
    AuthorizeLinkedEntityForUpdateChartStep authorizeLinked,
    PersistUpdateDashboardChartStep persist) : IChainProvider<UpdateDashboardChartContext>
{
    public IStep<UpdateDashboardChartContext> GetChain()
    {
        authorizeAndLoad.SetNext(validate).SetNext(authorizeLinked).SetNext(persist);
        return authorizeAndLoad;
    }
}
