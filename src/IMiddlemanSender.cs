namespace sge.framework.middleman;

public interface IMiddlemanSender
{
    Task<TResponse> Send<TResponse>(
        IMiddlemanRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}