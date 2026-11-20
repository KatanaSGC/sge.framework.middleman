namespace sge.framework.middleman;

public interface IMiddlemanHandler<in TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}