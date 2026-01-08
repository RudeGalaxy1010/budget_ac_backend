using budget_ac_backend.App.Data;
using budget_ac_backend.App.Operations.Requests.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.Operations.Requests;

public class CreateOperationRequest(
    IOperationRepository operationRepository,
    IValidator<CreateOperationRequestData> validator)
    : IdentifiedRequest<CreateOperationRequestData> {
    private readonly IOperationRepository _operationRepository = operationRepository.ThrowIfArgumentNull();
    private readonly IValidator<CreateOperationRequestData> _validator = validator.ThrowIfArgumentNull();

    protected override async Task<IResult> OnHandle(int userId, CreateOperationRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        Operation operation = await _operationRepository.CreateOperation(
            userId,
            request.CategoryName,
            request.Money,
            request.Date);

        return Results.Ok(new {
            Id = operation.Id,
            CategoryName = operation.Category.Name,
            Money = operation.Money,
            Date = DateTime.SpecifyKind(operation.Date, DateTimeKind.Utc),
        });
    }
}
