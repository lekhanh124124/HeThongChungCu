namespace HeThongChungCu.Application.Common.Messaging;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
