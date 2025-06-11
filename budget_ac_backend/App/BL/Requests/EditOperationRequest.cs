using budget_ac_backend.App.BL.Requests.Data;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.BL.Requests;

public class EditOperationRequest : IdentifiedRequest<EditOperationRequestData> {
    private readonly IOperationRepository _operationRepository;
    private readonly IValidator<EditOperationRequestData> _validator;

    public EditOperationRequest(
        IOperationRepository operationRepository,
        IValidator<EditOperationRequestData> validator) {
        _operationRepository = operationRepository.ThrowIfArgumentNull();
        _validator = validator.ThrowIfArgumentNull();
    }

    protected override async Task<IResult> OnHandle(int userId, EditOperationRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        Operation? operation = await _operationRepository.GetOperationById(request.OperationId);

        if (operation == null || operation.UserId != userId) {
            return Results.BadRequest(new { error = ErrorMessages.OperationNotFound });
        }

        if (operation.CategoryId == request.CategoryId
            && operation.Money == request.Money
            && operation.Date == request.Date
            && operation.Description == request.Description) {
            return Results.BadRequest(new { error = ErrorMessages.OperationHasNoChanges });
        }

        await _operationRepository.UpdateOperation(request.OperationId, request.CategoryId, request.Money, request.Date, request.Description);
        return Results.Ok();
    }
}