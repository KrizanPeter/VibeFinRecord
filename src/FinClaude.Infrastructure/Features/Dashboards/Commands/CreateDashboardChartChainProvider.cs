using FinClaude.Application.Common.Chain;
using FinClaude.Application.Features.Dashboards.Commands.CreateDashboardChart;
using FinClaude.Infrastructure.Features.Dashboards.Commands.Steps;

namespace FinClaude.Infrastructure.Features.Dashboards.Commands;

public class CreateDashboardChartChainProvider(
    ValidateCreateDashboardChartStep validate,
    AuthorizeLinkedEntityForCreateChartStep authorizeLinked,
    PersistCreateDashboardChartStep persist) : IChainProvider<CreateDashboardChartContext>
{
    public IStep<CreateDashboardChartContext> GetChain()
    {
        validate.SetNext(authorizeLinked).SetNext(persist);
        return validate;
    }
}
