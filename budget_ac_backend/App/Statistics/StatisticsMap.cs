using budget_ac_backend.App.Operations.Requests;
using budget_ac_backend.App.Operations.Requests.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Statistics.Requests;
using budget_ac_backend.App.Statistics.Requests.Data;
using budget_ac_backend.App.Utils;
using FluentValidation;

namespace budget_ac_backend.App.Statistics;

public class StatisticsMap(WebApplication app) {
    private const string GetYearlyStatisticsRoute = "/statistics/yearly";
    private const string GetTopCategoriesRoute = "/statistics/topcategories";

    private readonly WebApplication _app = app.ThrowIfArgumentNull();

    public void MapRoutes() {
        _app.MapPost(GetYearlyStatisticsRoute, GetYearlyStatistics);
        _app.MapPost(GetTopCategoriesRoute, GetTopCategories);
    }

    private async static Task<IResult> GetYearlyStatistics(
        GetPeriodStatisticsRequestData request,
        HttpContext context,
        IValidator<GetPeriodStatisticsRequestData> getPeriodStatisticsRequestDataValidator,
        IOperationRepository operationRepository) {
        GetPeriodStatisticsRequest getPeriodStatisticsRequest = new GetPeriodStatisticsRequest(
            operationRepository,
            getPeriodStatisticsRequestDataValidator);

        return await getPeriodStatisticsRequest.Handle(context, request);
    }

    private async static Task<IResult> GetTopCategories(
        GetTopCategoriesRequestData request,
        HttpContext context,
        IValidator<GetTopCategoriesRequestData> getTopCategoriesRequestDataValidator,
        IOperationRepository operationRepository) {
        GetTopCategoriesRequest getTopCategoriesRequest = new GetTopCategoriesRequest(
            operationRepository,
            getTopCategoriesRequestDataValidator);
        return await getTopCategoriesRequest.Handle(context, request);
    }
}
