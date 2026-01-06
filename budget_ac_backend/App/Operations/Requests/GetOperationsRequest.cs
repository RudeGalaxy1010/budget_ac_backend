using budget_ac_backend.App.Data;
using budget_ac_backend.App.Operations.Requests.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.Operations.Requests;

public class GetOperationsRequest : IdentifiedRequest<GetOperationsRequestData> {
    private readonly IOperationRepository _operationRepository;
    private readonly IValidator<GetOperationsRequestData> _validator;

    public GetOperationsRequest(
        IOperationRepository operationRepository,
        IValidator<GetOperationsRequestData> validator) {
        _operationRepository = operationRepository.ThrowIfArgumentNull();
        _validator = validator.ThrowIfArgumentNull();
    }

    protected override async Task<IResult> OnHandle(int userId, GetOperationsRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        List<Operation> operations = await _operationRepository.GetOperations(userId, request.From, request.To);
        return Results.Ok(operations.Select(o => new {
            Id = o.Id,
            Money = o.Money,
            Date = o.Date,
            CategoryName = o.Category.Name
        }));
    }
}
