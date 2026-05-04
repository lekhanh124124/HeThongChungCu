using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var isCommand = request.GetType().GetInterfaces().Any(i => 
            i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ICommand<>) || i.GetGenericTypeDefinition() == typeof(IRequest<>))
            && request.GetType().Name.EndsWith("Command")); // Thêm check Name để chắc chắn hơn cho Command

        if (!isCommand)
        {
            return await next(cancellationToken);
        }

        var requestName = typeof(TRequest).Name;

        return await _unitOfWork.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Starting transaction for {RequestName}", requestName);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next(cancellationToken);

                if (response is Result { IsFailure: true } result)
                {
                    _logger.LogWarning("Command {RequestName} failed with error. Rolling back transaction. Errors: {Errors}", 
                        requestName, string.Join(", ", result.Errors.Select(e => e.Description)));
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return response;
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                
                _logger.LogInformation("Transaction committed for {RequestName}", requestName);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during transaction for {RequestName}. Rolling back.", requestName);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        });
    }
}
