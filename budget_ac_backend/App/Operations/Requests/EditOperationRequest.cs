using budget_ac_backend.App.Data;
using budget_ac_backend.App.Operations.Requests.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.Operations.Requests;

public class EditOperationRequest(
    IOperationRepository operationRepository,
    IValidator<EditOperationRequestData> validator)
    : IdentifiedRequest<EditOperationRequestData> {
    private readonly IOperationRepository _operationRepository = operationRepository.ThrowIfArgumentNull();
    private readonly IValidator<EditOperationRequestData> _validator = validator.ThrowIfArgumentNull();

    protected override async Task<IResult> OnHandle(int userId, EditOperationRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        Operation? operation = await _operationRepository.GetOperation(request.Id);

        if (operation == null || operation.UserId != userId) {
            return Results.BadRequest(new { error = ErrorMessages.OperationNotFound });
        }

        if (operation.Category.Name == request.CategoryName
            && operation.Money == request.Money
            && operation.Date == request.Date) {
            return Results.BadRequest(new { error = ErrorMessages.OperationHasNoChanges });
        }

        await _operationRepository.UpdateOperation(operation, request.CategoryName, request.Money, request.Date);
        return Results.Ok(new {
            Id = operation.Id,
            CategoryName = operation.Category.Name,
            Money = operation.Money,
            Date = DateTime.SpecifyKind(operation.Date, DateTimeKind.Utc),
        });
    }
}
