using budget_ac_backend.App.Data;
using budget_ac_backend.App.Operations.Requests;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Statistics.Requests.Data;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.Statistics.Requests;

public class GetTopCategoriesRequest(IOperationRepository repository, IValidator<GetTopCategoriesRequestData> validator)
    : IdentifiedRequest<GetTopCategoriesRequestData> {
    private readonly IValidator<GetTopCategoriesRequestData> _validator = validator.ThrowIfArgumentNull();
    private readonly IOperationRepository _repository = repository.ThrowIfArgumentNull();

    protected override async Task<IResult> OnHandle(int userId, GetTopCategoriesRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        List<TopCategorySummary> topCategories = await _repository.GetTopCategories(userId, request.Year, request.Count);
        return Results.Ok(topCategories.Select(value => new {
            Name = value.Name,
            OperationsCount = value.OperationCount,
            Outcome = value.Outcome
        }));
    }
}
