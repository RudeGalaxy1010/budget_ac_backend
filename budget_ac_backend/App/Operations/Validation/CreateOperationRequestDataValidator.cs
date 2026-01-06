using budget_ac_backend.App.Operations.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.Operations.Validation;

public class CreateOperationRequestDataValidator : AbstractValidator<CreateOperationRequestData> {
    public CreateOperationRequestDataValidator() {
        RuleFor(x => x.CategoryName)
            .NotNull().WithMessage("Category cannot be null.");

        RuleFor(x => x.Money)
            .NotNull().WithMessage("Money cannot be null.")
            .NotEqual(0).WithMessage("Money cannot be zero.");

        RuleFor(x => x.Date)
            .NotNull()
            .WithMessage("Description must not exceed 256 characters.");
    }
}
