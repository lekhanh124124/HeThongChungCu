using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Common.Behaviors;

public class MaintenanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMaintenanceService _maintenanceService;
    private readonly ILogger<MaintenanceBehavior<TRequest, TResponse>> _logger;

    public MaintenanceBehavior(IMaintenanceService maintenanceService, ILogger<MaintenanceBehavior<TRequest, TResponse>> logger)
    {
        _maintenanceService = maintenanceService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Bỏ qua kiểm tra nếu không ở chế độ bảo trì hoặc đây là chính lệnh Restore
        if (!_maintenanceService.IsMaintenanceActive() || requestName.Equals("RestoreBackupCommand"))
        {
            return await next(cancellationToken);
        }

        _logger.LogWarning("Command/Query {RequestName} has been blocked because the system is in Maintenance Mode.", requestName);

        var error = new Error("System.Maintenance", "Hệ thống đang bảo trì để khôi phục dữ liệu nghiệp vụ.");

        // 1. Kiểm tra xem kiểu trả về (TResponse) có là Result<T> hay không để trả về Result.Failure sạch sẽ
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];
            var failureMethod = typeof(Result).GetMethods()
                .First(m => m.Name == "Failure" 
                            && m.IsGenericMethod 
                            && m.GetParameters().Length == 1 
                            && m.GetParameters()[0].ParameterType == typeof(Error));
            var genericFailureMethod = failureMethod.MakeGenericMethod(valueType);
            
            var result = genericFailureMethod.Invoke(null, new object[] { error });
            return (TResponse)result!;
        }
        
        // 2. Kiểm tra xem kiểu trả về có phải là Result thường không
        if (typeof(TResponse) == typeof(Result))
        {
            var result = Result.Failure(error);
            return (TResponse)(object)result;
        }

        // 3. Với các kiểu trả về khác (không bọc trong Result), ta ném exception để pipeline hoặc background job tự xử lý
        throw new BusinessException("Hệ thống đang bảo trì để khôi phục dữ liệu nghiệp vụ. Mọi tác vụ bị tạm dừng.", "System.Maintenance");
    }
}
