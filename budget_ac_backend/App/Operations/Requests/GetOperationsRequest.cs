using budget_ac_backend.App.Data;
using budget_ac_backend.App.Operations.Requests.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.Operations.Requests;

public class GetOperationsRequest(
    IOperationRepository operationRepository,
    IValidator<GetOperationsRequestData> validator)
    : IdentifiedRequest<GetOperationsRequestData> {
    private readonly IOperationRepository _operationRepository = operationRepository.ThrowIfArgumentNull();
    private readonly IValidator<GetOperationsRequestData> _validator = validator.ThrowIfArgumentNull();

    protected override async Task<IResult> OnHandle(int userId, GetOperationsRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        List<Operation> operations = await _operationRepository.GetOperations(userId, request.From, request.To);
        return Results.Ok(operations.Select(operation => new {
            Id = operation.Id,
            Money = operation.Money,
            Date = DateTime.SpecifyKind(operation.Date, DateTimeKind.Utc),
            CategoryName = operation.Category.Name
        }));
    }
}
