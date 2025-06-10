using budget_ac_backend.App.BL.Requests.Data;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.BL.Requests;

public class EditCategoryRequest : IdentifiedRequest<EditCategoryRequestData> {
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<EditCategoryRequestData> _validator;

    public EditCategoryRequest(
        ICategoryRepository categoryRepository,
        IValidator<EditCategoryRequestData> validator) {
        _categoryRepository = categoryRepository.ThrowIfArgumentNull();
        _validator = validator.ThrowIfArgumentNull();
    }

    protected override async Task<IResult> OnHandle(int userId, EditCategoryRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        Category? category = await _categoryRepository.GetCategoryById(request.CategoryId);

        if (category == null) {
            return Results.BadRequest(new { error = ErrorMessages.CategoryNotFound });
        }

        if (category.UserId != userId) {
            return Results.BadRequest(new { error = ErrorMessages.UserNotFound });
        }

        if (category.Name == request.Name && category.Description == request.Description) {
            return Results.BadRequest(new { error = ErrorMessages.CategoryHasNoChanges });
        }

        category.Name = request.Name;
        category.Description = request.Description;
        await _categoryRepository.UpdateCategory(category);

        return Results.Ok();
    }
}