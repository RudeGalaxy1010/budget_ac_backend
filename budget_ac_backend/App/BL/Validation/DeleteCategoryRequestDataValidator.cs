using budget_ac_backend.App.BL.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.BL.Validation;

public class DeleteCategoryRequestDataValidator : AbstractValidator<DeleteCategoryRequestData> {
    public DeleteCategoryRequestDataValidator() {
        RuleFor(x => x.CategoryId)
            .NotNull().WithMessage("CategoryId cannot be null");
    }
}