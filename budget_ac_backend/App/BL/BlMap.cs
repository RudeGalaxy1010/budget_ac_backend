using budget_ac_backend.App.BL.Requests;
using budget_ac_backend.App.BL.Requests.Data;
using budget_ac_backend.App.BL.Validation;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;

namespace budget_ac_backend.App.BL;

public class BlMap {
    private const string CreateOperation = "/operations/create";
    private const string GetOperations = "/operations/get";
    private const string EditOperation = "/operations/edit";
    private const string DeleteOperation = "/operations/delete";

    private const string GetYearlyStatistics = "/statistics/yearly";
    private const string GetTopCategories = "/statistics/topcategories";

    private readonly WebApplication _app;

    private readonly CreateOperationRequest _createOperationRequest;
    private readonly GetOperationsRequest _getOperationsRequest;
    private readonly EditOperationRequest _editOperationRequest;
    private readonly DeleteOperationRequest _deleteOperationRequest;

    private readonly GetPeriodStatisticsRequest _getPeriodStatisticsRequest;
    private readonly GetTopCategoriesRequest _getTopCategoriesRequest;

    public BlMap(WebApplication app, IUserRepository userRepository, IOperationRepository operationRepository) {
        _app = app.ThrowIfArgumentNull();
        userRepository.ThrowIfArgumentNull();
        operationRepository.ThrowIfArgumentNull();

        // Operations
        IValidator<CreateOperationsRequestData> createOperationRequestDataValidator = new CreateOperationRequestDataValidator();
        _createOperationRequest = new CreateOperationRequest(operationRepository, createOperationRequestDataValidator);

        IValidator<GetOperationsRequestData> getOperationsRequestDataValidator = new GetOperationsRequestDataValidator();
        _getOperationsRequest = new GetOperationsRequest(operationRepository, getOperationsRequestDataValidator);

        IValidator<EditOperationRequestData> editOperationRequestDataValidator = new EditOperationRequestDataValidator();
        _editOperationRequest = new EditOperationRequest(operationRepository, editOperationRequestDataValidator);

        IValidator<DeleteOperationRequestData> deleteOperationRequestDataValidator = new DeleteOperationRequestDataValidator();
        _deleteOperationRequest = new DeleteOperationRequest(operationRepository, deleteOperationRequestDataValidator);

        IValidator<GetPeriodStatisticsRequestData> getPeriodStatisticsRequestDataValidator = new GetPeriodStatisticsRequestDataValidator();
        _getPeriodStatisticsRequest = new GetPeriodStatisticsRequest(operationRepository, getPeriodStatisticsRequestDataValidator);

        IValidator<GetTopCategoriesRequestData> getTopCategoriesRequestDataValidator = new GetTopCategoriesRequestDataValidator();
        _getTopCategoriesRequest = new GetTopCategoriesRequest(operationRepository, getTopCategoriesRequestDataValidator);
    }

    public void MapRequests() {
        _app.MapPost(CreateOperation, (HttpContext context, CreateOperationsRequestData request) =>
            _createOperationRequest.Handle(context, request));

        _app.MapPost(GetOperations, (HttpContext context, GetOperationsRequestData request) =>
            _getOperationsRequest.Handle(context, request));

        _app.MapPost(EditOperation, (HttpContext context, EditOperationRequestData request) =>
            _editOperationRequest.Handle(context, request));

        _app.MapPost(DeleteOperation, (HttpContext context, DeleteOperationRequestData request) =>
            _deleteOperationRequest.Handle(context, request));

        _app.MapPost(GetYearlyStatistics, (HttpContext context, GetPeriodStatisticsRequestData request) =>
            _getPeriodStatisticsRequest.Handle(context, request));

        _app.MapPost(GetTopCategories, (HttpContext context, GetTopCategoriesRequestData request) =>
            _getTopCategoriesRequest.Handle(context, request));
    }
}
