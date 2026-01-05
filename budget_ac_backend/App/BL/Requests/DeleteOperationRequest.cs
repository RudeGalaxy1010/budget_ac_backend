using budget_ac_backend.App.BL.Requests.Data;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.BL.Requests;

public class DeleteOperationRequest : IdentifiedRequest<DeleteOperationRequestData> {
    private readonly IOperationRepository _operationRepository;
    private readonly IValidator<DeleteOperationRequestData> _validator;

    public DeleteOperationRequest(
        IOperationRepository operationRepository,
        IValidator<DeleteOperationRequestData> validator) {
        _operationRepository = operationRepository.ThrowIfArgumentNull();
        _validator = validator.ThrowIfArgumentNull();
    }

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
