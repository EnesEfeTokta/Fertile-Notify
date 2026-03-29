namespace FertileNotify.Application.UseCases.Common
{
    public interface ICommand : IRequest<Unit>;

    public interface ICommand<out TResponse> : IRequest<TResponse>;

    public interface IQuery<out TResponse> : IRequest<TResponse>;

    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Unit>
        where TCommand : ICommand;

    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>;

    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>;
}
