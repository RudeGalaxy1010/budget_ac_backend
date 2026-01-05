using budget_ac_backend.App.BL.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.BL.Validation;

public class EditOperationRequestDataValidator : AbstractValidator<EditOperationRequestData> {
    public EditOperationRequestDataValidator() {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("OperationId cannot be null.");

        // RuleFor(x => x.CategoryId)
        //     .NotNull().WithMessage("CategoryId cannot be null.");

        RuleFor(x => x.Money)
            .NotNull().WithMessage("Money cannot be null.")
            .NotEqual(0).WithMessage("Money cannot be zero.");

        RuleFor(x => x.Date)
            .NotNull().WithMessage("Date cannot be null.");

        // RuleFor(x => x.Description)
        //     .MaximumLength(256).WithMessage("Description must not exceed 256 characters.");
    }
}