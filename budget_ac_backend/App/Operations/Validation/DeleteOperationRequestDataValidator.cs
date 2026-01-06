using budget_ac_backend.App.Operations.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.Operations.Validation;

public class DeleteOperationRequestDataValidator : AbstractValidator<DeleteOperationRequestData> {
    public DeleteOperationRequestDataValidator() {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("OperationId cannot be null.");
    }
}