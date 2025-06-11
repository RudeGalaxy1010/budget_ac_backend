using budget_ac_backend.App.BL.Requests.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.BL.Requests;

public class CreateOperationRequest : IdentifiedRequest<CreateOperationsRequestData> {
    private readonly IOperationRepository _operationRepository;
    private readonly IValidator<CreateOperationsRequestData> _validator;

    public CreateOperationRequest(
        IOperationRepository operationRepository,
        IValidator<CreateOperationsRequestData> validator) {
        _operationRepository = operationRepository.ThrowIfArgumentNull();
        _validator = validator.ThrowIfArgumentNull();
    }

    protected override async Task<IResult> OnHandle(int userId, CreateOperationsRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        await _operationRepository.CreateOperation(userId, request.CategoryId, request.Money, DateTime.UtcNow, request.Description);
        return Results.Ok();
    }
}