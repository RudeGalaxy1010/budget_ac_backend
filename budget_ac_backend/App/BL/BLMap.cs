using budget_ac_backend.App.BL.Requests;
using budget_ac_backend.App.BL.Requests.Data;
using budget_ac_backend.App.BL.Validation;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;

namespace budget_ac_backend.App.BL;

public class BLMap {
    private const string CreateCategory = "/categories/create";
    private const string EditCategory = "/categories/edit";
    private const string GetCategory = "/categories/get";
    private const string DeleteCategory = "/categories/delete";

    private readonly WebApplication _app;

    private readonly CreateCategoryRequest _createCategoryRequest;
    private readonly EditCategoryRequest _editCategoryRequest;
    private readonly GetCategoriesRequest _getCategoriesRequest;
    private readonly DeleteCategoryRequest _deleteCategoryRequest;

    public BLMap(WebApplication app, IUserRepository userRepository, ICategoryRepository categoryRepository) {
        _app = app.ThrowIfArgumentNull();
        userRepository.ThrowIfArgumentNull();
        categoryRepository.ThrowIfArgumentNull();

        IValidator<CreateCategoryRequestData> createCategoryRequestDataValidator = new CreateCategoryRequestDataValidator();
        _createCategoryRequest = new CreateCategoryRequest(categoryRepository, createCategoryRequestDataValidator);

        IValidator<EditCategoryRequestData> editCategoryRequestDataValidator = new EditCategoryRequestDataValidator();
        _editCategoryRequest = new EditCategoryRequest(categoryRepository, editCategoryRequestDataValidator);

        IValidator<GetCategoriesRequestData> getCategoriesRequestDataValidator = new GetCategoriesRequestDataValidator();
        _getCategoriesRequest = new GetCategoriesRequest(categoryRepository, getCategoriesRequestDataValidator);

        IValidator<DeleteCategoryRequestData> deleteCategoryRequestDataValidator = new DeleteCategoryRequestDataValidator();
        _deleteCategoryRequest = new DeleteCategoryRequest(categoryRepository, deleteCategoryRequestDataValidator);
    }

    public void MapRequests() {
        _app.MapPost(CreateCategory, (HttpContext context, CreateCategoryRequestData request) =>
            _createCategoryRequest.Handle(context, request));

        _app.MapPost(EditCategory, (HttpContext context, EditCategoryRequestData request) =>
            _editCategoryRequest.Handle(context, request));

        _app.MapPost(GetCategory, (HttpContext context, GetCategoriesRequestData request) =>
            _getCategoriesRequest.Handle(context, request));

        _app.MapPost(DeleteCategory, (HttpContext context, DeleteCategoryRequestData request) =>
            _deleteCategoryRequest.Handle(context, request));
    }
}