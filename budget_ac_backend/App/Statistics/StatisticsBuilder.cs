using budget_ac_backend.App.Operations.Requests.Data;
using budget_ac_backend.App.Operations.Validation;
using budget_ac_backend.App.Statistics.Requests.Data;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace budget_ac_backend.App.Statistics;

public static class StatisticsBuilder {
    public static void AddStatistics(this WebApplicationBuilder builder) {
        builder.Services.TryAddSingleton<IValidator<GetPeriodStatisticsRequestData>, GetPeriodStatisticsRequestDataValidator>();
        builder.Services.TryAddSingleton<IValidator<GetTopCategoriesRequestData>, GetTopCategoriesRequestDataValidator>();
    }
}
