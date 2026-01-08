using budget_ac_backend.App.Data;
using budget_ac_backend.App.Operations.Requests.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.Operations.Requests;

public class DeleteOperationRequest(
    IOperationRepository operationRepository,
    IValidator<DeleteOperationRequestData> validator)
    : IdentifiedRequest<DeleteOperationRequestData> {
    private readonly IOperationRepository _operationRepository = operationRepository.ThrowIfArgumentNull();
    private readonly IValidator<DeleteOperationRequestData> _validator = validator.ThrowIfArgumentNull();

    protected override async Task<IResult> OnHandle(int userId, DeleteOperationRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        Operation? operation = await _operationRepository.GetOperation(request.Id);

        if (operation == null || operation.UserId != userId) {
            return Results.BadRequest(new { error = ErrorMessages.OperationNotFound });
        }

        await _operationRepository.DeleteOperation(operation);
        return Results.Ok();
    }
}
