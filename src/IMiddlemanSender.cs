namespace sge.framework.middleman;

public interface IMiddlemanSender
{
    Task<TResponse> Send<TRequest, TResponse>(TRequest request);
}