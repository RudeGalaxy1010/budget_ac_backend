using budget_ac_backend.App.BL.Requests.Data;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.BL.Requests;

public class GetPeriodStatisticsRequest(
    IOperationRepository operationRepository,
    IValidator<GetPeriodStatisticsRequestData> validator) : IdentifiedRequest<GetPeriodStatisticsRequestData> {
    private readonly IOperationRepository _operationRepository = operationRepository.ThrowIfArgumentNull();
    private readonly IValidator<GetPeriodStatisticsRequestData> _validator = validator.ThrowIfArgumentNull();

    protected override async Task<IResult> OnHandle(int userId, GetPeriodStatisticsRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        Dictionary<int, PeriodStatistics> statistics = await _operationRepository.GetPeriodStatistics(userId, request.Year);
        return Results.Ok(statistics.Select(value => new {
            Month = value.Key,
            Income = value.Value.Income,
            Outcome = Math.Abs(value.Value.Outcome),
        }));
    }
}
