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

    private const string CreateOperation = "/operations/create";
    private const string GetOperations = "/operations/get";
    private const string EditOperation = "/operations/edit";
    private const string DeleteOperation = "/operations/delete";

    private readonly WebApplication _app;

    private readonly CreateCategoryRequest _createCategoryRequest;
    private readonly EditCategoryRequest _editCategoryRequest;
    private readonly GetCategoriesRequest _getCategoriesRequest;
    private readonly DeleteCategoryRequest _deleteCategoryRequest;

    private readonly CreateOperationRequest _createOperationRequest;
    private readonly GetOperationsRequest _getOperationsRequest;
    private readonly EditOperationRequest _editOperationRequest;
    private readonly DeleteOperationRequest _deleteOperationRequest;

    public BLMap(WebApplication app, IUserRepository userRepository, ICategoryRepository categoryRepository, IOperationRepository operationRepository) {
        _app = app.ThrowIfArgumentNull();
        userRepository.ThrowIfArgumentNull();
        categoryRepository.ThrowIfArgumentNull();
        operationRepository.ThrowIfArgumentNull();

        // Categories
        IValidator<CreateCategoryRequestData> createCategoryRequestDataValidator = new CreateCategoryRequestDataValidator();
        _createCategoryRequest = new CreateCategoryRequest(categoryRepository, createCategoryRequestDataValidator);

        IValidator<EditCategoryRequestData> editCategoryRequestDataValidator = new EditCategoryRequestDataValidator();
        _editCategoryRequest = new EditCategoryRequest(categoryRepository, editCategoryRequestDataValidator);

        IValidator<GetCategoriesRequestData> getCategoriesRequestDataValidator = new GetCategoriesRequestDataValidator();
        _getCategoriesRequest = new GetCategoriesRequest(categoryRepository, getCategoriesRequestDataValidator);

        IValidator<DeleteCategoryRequestData> deleteCategoryRequestDataValidator = new DeleteCategoryRequestDataValidator();
        _deleteCategoryRequest = new DeleteCategoryRequest(categoryRepository, deleteCategoryRequestDataValidator);

        // Operations
        IValidator<CreateOperationsRequestData> createOperationRequestDataValidator = new CreateOperationRequestDataValidator();
        _createOperationRequest = new CreateOperationRequest(operationRepository, createOperationRequestDataValidator);

        IValidator<GetOperationsRequestData> getOperationsRequestDataValidator = new GetOperationsRequestDataValidator();
        _getOperationsRequest = new GetOperationsRequest(operationRepository, getOperationsRequestDataValidator);

        IValidator<EditOperationRequestData> editOperationRequestDataValidator = new EditOperationRequestDataValidator();
        _editOperationRequest = new EditOperationRequest(operationRepository, editOperationRequestDataValidator);

        IValidator<DeleteOperationRequestData> deleteOperationRequestDataValidator = new DeleteOperationRequestDataValidator();
        _deleteOperationRequest = new DeleteOperationRequest(operationRepository, deleteOperationRequestDataValidator);
    }

    public void MapRequests() {
        // Categories
        _app.MapPost(CreateCategory, (HttpContext context, CreateCategoryRequestData request) =>
            _createCategoryRequest.Handle(context, request));

        _app.MapPost(EditCategory, (HttpContext context, EditCategoryRequestData request) =>
            _editCategoryRequest.Handle(context, request));

        _app.MapPost(GetCategory, (HttpContext context, GetCategoriesRequestData request) =>
            _getCategoriesRequest.Handle(context, request));

        _app.MapPost(DeleteCategory, (HttpContext context, DeleteCategoryRequestData request) =>
            _deleteCategoryRequest.Handle(context, request));

        // Operations
        _app.MapPost(CreateOperation, (HttpContext context, CreateOperationsRequestData request) =>
            _createOperationRequest.Handle(context, request));

        _app.MapPost(GetOperations, (HttpContext context, GetOperationsRequestData request) =>
            _getOperationsRequest.Handle(context, request));

        _app.MapPost(EditOperation, (HttpContext context, EditOperationRequestData request) =>
            _editOperationRequest.Handle(context, request));

        _app.MapPost(DeleteOperation, (HttpContext context, DeleteOperationRequestData request) =>
            _deleteOperationRequest.Handle(context, request));
    }
}