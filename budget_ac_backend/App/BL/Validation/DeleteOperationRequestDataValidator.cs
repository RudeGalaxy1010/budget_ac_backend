using budget_ac_backend.App.BL.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.BL.Validation;

public class DeleteOperationRequestDataValidator : AbstractValidator<DeleteOperationRequestData> {
    public DeleteOperationRequestDataValidator() {
        RuleFor(x => x.OperationId)
            .NotNull().WithMessage("OperationId cannot be null.");
    }
}